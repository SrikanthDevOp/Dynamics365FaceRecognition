﻿using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hsl.CognitiveServices.Demo.UserControl
{
    /// <summary>
    /// Interaction logic for FaceIdentify.xaml
    /// </summary>
    public partial class FaceIdentify : ITabbed, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Description dependency property
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(FaceIdentify));


        /// <summary>
        /// Faces to identify
        /// </summary>
        private ObservableCollection<Face> _faces = new ObservableCollection<Face>();

        /// <summary>
        /// Person database
        /// </summary>
        private ObservableCollection<Person> _persons = new ObservableCollection<Person>();

        /// <summary>
        /// User picked image file
        /// </summary>
        private ImageSource _selectedFile;

        /// <summary>
        /// max concurrent process number for client query.
        /// </summary>
        private int _maxConcurrentProcesses;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets description
        /// </summary>
        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }

            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets FaceId
        /// </summary>
        public string FaceId { get; set; }
        

        /// <summary>
        /// Gets or sets group id.
        /// </summary>
        public string PersonGroupId = "dynamics365contacts";

        /// <summary>
        /// Gets constant maximum image size for rendering detection result
        /// </summary>
        public int MaxImageSize
        {
            get
            {
                return 186;
            }
        }

        /// <summary>
        /// Gets person database
        /// </summary>
        public ObservableCollection<Person> Persons
        {
            get
            {
                return _persons;
            }
        }

        /// <summary>
        /// Gets or sets user picked image file
        /// </summary>
        public ImageSource SelectedFile
        {
            get
            {
                return _selectedFile;
            }

            set
            {
                _selectedFile = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedFile"));
                }
            }
        }

        /// <summary>
        /// Implement INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets faces to identify
        /// </summary>
        public ObservableCollection<Face> TargetFaces
        {
            get
            {
                return _faces;
            }
        }

        private string  ImagePath{get;set;}
        private List<Contact> IdentifiedContacts { get; set; }

        private string strSubscriptionKey = string.Empty;
        private string strEndpoint = string.Empty;

        #endregion Properties


        public FaceIdentify()
        {
            InitializeComponent();
            _maxConcurrentProcesses = 4;
        }

        /// <summary>
        /// This event will be fired when user will click close button
        /// </summary>
        public event Close CloseInitiated;

        #region Events
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseInitiated?.Invoke(this, new EventArgs());
        }
        private void BtnNextKeyManagement_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSubscriptionKey.Text) || string.IsNullOrEmpty(txtEndpoint.Text))
            {

            }
            else
            {
                this.strSubscriptionKey = txtSubscriptionKey.Text;
                this.strEndpoint = txtEndpoint.Text;
                gridFaceAPIKeyManagement.Visibility = Visibility.Hidden;
                gridTrainPersonGroup.Visibility = Visibility.Visible;
            }
        }

        private void BtnTrain_Click(object sender, RoutedEventArgs e)
        {
            bool blnTrainingSuccess = false;
            try
            {
                List<Contact> lstContacts = new List<Contact>();
                // Get the Contacts Images from CRM.
                string strContactsImageQuery = @"<fetch mapping='logical' output-format='xml-platform' distinct='false'>
                          <entity name='contact'>
                            <attribute name='fullname'/>
                            <attribute name='contactid'/>
                            <attribute name='entityimage'/>
                            <filter type='and'>
                            <condition attribute='statecode' operator='eq' value='0'/>
                            <condition attribute='entityimage' operator='not-null'/>
                            </filter>
                          </entity>
                        </fetch>";
                EntityCollection entColContacts = CrmHelper._serviceProxy.RetrieveMultiple(new FetchExpression(strContactsImageQuery));
                if (entColContacts == null || entColContacts.Entities.Count == 0)
                {
                    return;
                }
                List<Contact> lstFilteredContacts = entColContacts.Entities
                    .Select(ent => new Contact
                    {
                        FullName = ent.Attributes["fullname"].ToString(),
                        ImageBytes = ent.Attributes["entityimage"] as byte[],
                        Id = ent.Attributes["contactid"].ToString()
                    }).ToList();
                CreatePersonGroup(lstFilteredContacts);
                blnTrainingSuccess = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                blnTrainingSuccess = false;
            }
            if (blnTrainingSuccess)
            {
                gridTrainPersonGroup.Visibility = Visibility.Hidden;
                gridIdentifyPerson.Visibility = Visibility.Visible;
            }

        }
        #endregion

        private void CreatePersonGroup(List<Contact> lstContacts)
        {
            try
            {
                using (var faceServiceClient = new FaceServiceClient(this.strSubscriptionKey, this.strEndpoint))
                {
                    bool isGroupExists = CheckIfGroupExists();
                    if (!isGroupExists)
                    {
                        Task taskCreatePersonGroup = Task.Run(async () => await faceServiceClient.CreatePersonGroupAsync(this.PersonGroupId, "Dynamics 365 Contacts"));
                    }

                    lstContacts.ForEach(contact =>
                    {
                        Task<CreatePersonResult> taskCreatePerson = Task.Run(async () => await faceServiceClient.CreatePersonAsync(
                            // Id of the PersonGroup that the person belonged to
                            this.PersonGroupId,
                            contact.FullName));

                        while (!taskCreatePerson.IsCompleted)
                        {
                            taskCreatePerson.Wait();
                        }
                        CreatePersonResult person = taskCreatePerson.Result as CreatePersonResult;
                        using (Stream stream = new MemoryStream(contact.ImageBytes))
                        {
                            Task<AddPersistedFaceResult> taskPersisted =Task.Run(async()=> await faceServiceClient.AddPersonFaceAsync(PersonGroupId, person.PersonId, stream));
                            while(!taskPersisted.IsCompleted)
                            {
                                taskPersisted.Wait();
                            }
                            AddPersistedFaceResult persistedFaces = taskPersisted.Result as AddPersistedFaceResult;
                            Persons.Add(new Person()
                            {
                                PersonName = contact.FullName,
                                PersonId = person.PersonId.ToString(),
                                PersistedFaceId = persistedFaces.PersistedFaceId.ToString()
                            });
                        }
                    });
                    Task taskTrainPerson = Task.Run(async () => await faceServiceClient.TrainPersonGroupAsync(this.PersonGroupId));
                    while (!taskTrainPerson.IsCompleted)
                    {
                        taskTrainPerson.Wait();
                    }
                    while (true)
                    {
                        try
                        {
                            Task<TrainingStatus> taskTrainingStatus = Task.Run(async () => await faceServiceClient.GetPersonGroupTrainingStatusAsync(this.PersonGroupId));
                            if (taskTrainingStatus.Result.Status != Status.Running)
                            {
                                break;
                            }
                            else
                            {
                                taskTrainingStatus.Wait();
                            }
                        }
                        catch (FaceAPIException fEx)
                        {
                            continue;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CheckIfGroupExists()
        {
            // Test whether the group already exists
            bool groupExists = false;
            try
            {
                using (var faceServiceClient = new FaceServiceClient(this.strSubscriptionKey, this.strEndpoint))
                {

                    Task taskGetPerson = Task.Run(async () => await faceServiceClient.GetPersonGroupAsync(this.PersonGroupId));
                    while (true)
                    {
                        if (taskGetPerson.IsCompleted)
                        {
                            groupExists = true;
                            break;
                        }
                        else
                        {
                            taskGetPerson.Wait();
                        }
                    }

                }
            }
            catch (FaceAPIException ex)
            {
                if (ex.ErrorCode != "PersonGroupNotFound")
                {
                    groupExists = false;
                }
            }
            return groupExists;
        }

        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            // Show file picker
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image files(*.jpg, *.png, *.bmp, *.gif) | *.jpg; *.png; *.bmp; *.gif";
            var result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                btnIdentify.IsEnabled = true;
                // User picked one image
                // Clear previous detection and identification results
                TargetFaces.Clear();
                var pickedImagePath = dlg.FileName;
                this.ImagePath = pickedImagePath;
                var renderingImage = UIHelper.LoadImageAppliedOrientation(pickedImagePath);
                var imageInfo = UIHelper.GetImageInfoForRendering(renderingImage);
                SelectedFile = renderingImage;
                var sw = Stopwatch.StartNew();
                var faceServiceClient = new FaceServiceClient(this.strSubscriptionKey, this.strEndpoint);

                // Call detection REST API
                using (var fStream = File.OpenRead(pickedImagePath))
                {
                    try
                    {
                        var taskfaces = Task.Run(async()=> await faceServiceClient.DetectAsync(fStream));
                        while(!taskfaces.IsCompleted)
                        {
                            taskfaces.Wait();
                        }
                        var faces = taskfaces.Result.ToArray();
                    
                        foreach (var face in UIHelper.CalculateFaceRectangleForRendering(faces, MaxImageSize, imageInfo))
                        {
                            TargetFaces.Add(face);
                        }
                    }
                    catch (FaceAPIException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            GC.Collect();
        }

        private void BtnIdentify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Task<bool> taskIdentify = Task.Run((async () => await IdentifyFaces()));
                if (taskIdentify.Result)
                {
                    MessageBox.Show("Identification successfully completed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnIdentify.IsEnabled = false;
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.OriginalSource is Rectangle)
                {
                    Rectangle clickedRectangle = (Rectangle)e.OriginalSource;
                    string strFaceId = clickedRectangle.Uid;

                    Contact contact = this.IdentifiedContacts.Where(con => strFaceId == con.FaceId).FirstOrDefault();
                    if (contact != null)
                    {
                        string strContactUrl = CrmHelper._organisationUrl + $"main.aspx?etn=contact&pagetype=entityrecord&id={contact.Id}";
                        Process.Start("chrome.exe", "strContactUrl");
                    }
                }

                //// If the REST call has not completed, return from this method.
                //if (_faces == null)
                //    return;
                //var faces = _faces.ToArray();
                //// Find the mouse position relative to the image.
                //Point mouseXY = e.GetPosition(ImageDisplay);
                //ImageSource imageSource = ImageDisplay.Source;
                //BitmapSource bitmapSource = (BitmapSource)imageSource;
                //double dpi = bitmapSource.DpiX;
                //double resizeFactor = 96 / dpi;
                //// Scale adjustment between the actual size and displayed size.
                //var scale = ImageDisplay.ActualWidth / (bitmapSource.PixelWidth / resizeFactor);

                //for (int i = 0; i < faces.Length; ++i)
                //{
                //    var fr = faces[i].UIRect;
                //    double left = fr.X * scale;
                //    double top = fr.Y * scale;
                //    double width = fr.Width * scale;
                //    double height = fr.Height * scale;

                //    // Display the face description for this face if the mouse is over this face rectangle.
                //    if (mouseXY.X >= left && mouseXY.X <= left + width && mouseXY.Y >= top && mouseXY.Y <= top + height)
                //    {
                //        Contact contact = this.IdentifiedContacts.Where(con => faces[i].FaceId == con.FaceId).FirstOrDefault();
                //        if (contact != null)
                //        {
                //            string strContactUrl = CrmHelper._organisationUrl + $"main.aspx?etn=contact&pagetype=entityrecord&id={contact.Id}";
                //            Process.Start("chrome.exe", "strContactUrl");
                //        }
                //        break;
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task<bool> IdentifyFaces()
        {
            try
            {
                // Identify each face
                using (var faceServiceClient = new FaceServiceClient(this.strSubscriptionKey, this.strEndpoint))
                {
                    // Call identify REST API, the result contains identified person information
                    var identifyResult = await faceServiceClient.IdentifyAsync(TargetFaces.Select(ff => new Guid(ff.FaceId)).ToArray(), personGroupId: this.PersonGroupId);
                    this.IdentifiedContacts = new List<Contact>();
                    var faces = TargetFaces.ToArray();
                    for (int idx = 0; idx < faces.Length; idx++)
                    {
                        // Update identification result for rendering
                        var face = TargetFaces[idx];
                        var res = identifyResult[idx];

                        string contactName;
                        if (res.Candidates.Length > 0 /*&& Persons.Any(p => p.PersonId == res.Candidates[0].PersonId )*/ )
                        {
                            var person = await faceServiceClient.GetPersonAsync(this.PersonGroupId, res.Candidates[0].PersonId);
                            IdentifiedContacts.Add(new Contact()
                            {
                                FullName = person.Name,
                                Id = person.PersonId.ToString(),
                                FaceId =face.FaceId
                            });
                        }
                        else
                        {
                            IdentifiedContacts.Add(new Contact()
                            {
                                FullName = "UnKnown"
                            });
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void ItemsControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void gridIdentifyPerson_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            System.Windows.Controls.Image clickedImage = e.Source as System.Windows.Controls.Image;

            // Find the mouse position relative to the image.
            Point mouseXY = e.GetPosition(clickedImage);
            var renderingImage = UIHelper.LoadImageAppliedOrientation(this.ImagePath);
            var imageInfo = UIHelper.GetImageInfoForRendering(renderingImage);
            var imageWidth = imageInfo.Item1;
            var imageHeight = imageInfo.Item2;
            float ratio = (float)imageWidth / imageHeight;
            int uiWidth = 0;
            int uiHeight = 0;
            if (ratio > 1.0)
            {
                uiWidth = this.MaxImageSize;
                uiHeight = (int)(this.MaxImageSize / ratio);
            }
            else
            {
                uiHeight = this.MaxImageSize;
                uiWidth = (int)(ratio * uiHeight);
            }

            int uiXOffset = (MaxImageSize - uiWidth) / 2;
            int uiYOffset = (MaxImageSize - uiHeight) / 2;
            float scale = (float)uiWidth / imageWidth;
            foreach (var face in _faces)
            {
                var left = (face.Left - uiXOffset) / scale;
                var top= ((face.Top - uiYOffset) / scale);
                var height = face.Left / scale;
                var width = face.Width / scale;

                // Display the face description for this face if the mouse is over this face rectangle.
                if (mouseXY.X <= left && mouseXY.X <= left + width && mouseXY.Y >= top && mouseXY.Y <= top + height)
                {
                    Contact contact = this.IdentifiedContacts.Where(con => face.FaceId == con.FaceId).FirstOrDefault();
                    if (contact != null)
                    {
                        string strContactUrl = CrmHelper._organisationUrl + $"main.aspx?etn=contact&pagetype=entityrecord&id={contact.Id}";
                        Process.Start("chrome.exe", "strContactUrl");
                    }
                    break;
                }
            }
        }
    }

    public class Contact
    {
        public string FullName { get; set; }
        public byte[] ImageBytes { get; set; }
        public string Id { get; set; }
        public string FaceId { get; set; }

    }

    #region Nested Types

    /// <summary>
    /// Identification result for UI binding
    /// </summary>
    public class IdentificationResult : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Face to identify
        /// </summary>
        private Face _faceToIdentify;

        /// <summary>
        /// Identified person's name
        /// </summary>
        private string _name;

        #endregion Fields

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets face to identify
        /// </summary>
        public Face FaceToIdentify
        {
            get
            {
                return _faceToIdentify;
            }

            set
            {
                _faceToIdentify = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FaceToIdentify"));
                }
            }
        }

        /// <summary>
        /// Gets or sets identified person's name
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        #endregion Properties
    }
    /// <summary>
    /// Person structure for UI binding
    /// </summary>
    public class Person : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Person's faces from database
        /// </summary>
        private ObservableCollection<Face> _faces = new ObservableCollection<Face>();

        /// <summary>
        /// Person's id
        /// </summary>
        private string _personId;

        /// <summary>
        /// Person's name
        /// </summary>
        private string _personName;

        /// <summary>
        /// Person's Persisted face Id
        /// </summary>
        private string _persistedFaceId;

        #endregion Fields

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets person's faces from database
        /// </summary>
        public ObservableCollection<Face> Faces
        {
            get
            {
                return _faces;
            }

            set
            {
                _faces = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Faces"));
                }
            }
        }

        /// <summary>
        /// Gets or sets person's id
        /// </summary>
        public string PersonId
        {
            get
            {
                return _personId;
            }

            set
            {
                _personId = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PersonId"));
                }
            }
        }

        /// <summary>
        /// Gets or sets person's name
        /// </summary>
        public string PersonName
        {
            get
            {
                return _personName;
            }

            set
            {
                _personName = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PersonName"));
                }
            }
        }

        /// <summary>
        /// Gets or sets person's name
        /// </summary>
        public string PersistedFaceId
        {
            get
            {
                return _persistedFaceId;
            }

            set
            {
                _persistedFaceId = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PersistedFaceId"));
                }
            }
        }

        #endregion Properties
    }

    #endregion

}
