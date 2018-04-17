using Microsoft.ProjectOxford.Face;
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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hsl.CognitiveServices.Demo.UserControl
{
    /// <summary>
    /// Interaction logic for FaceIdentify.xaml
    /// </summary>
    public partial class FaceIdentify : ITabbed
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
                return 300;
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

        private async void CreatePersonGroup(List<Contact> lstContacts)
        {
            try
            {
                using (var faceServiceClient = new FaceServiceClient(this.strSubscriptionKey, this.strEndpoint))
                {
                    bool isGroupExists = await CheckIfGroupExists();
                    if (!isGroupExists)
                    {
                        await faceServiceClient.CreatePersonGroupAsync(this.PersonGroupId, "Dynamics 365 Contacts");
                    }

                    lstContacts.ForEach(async contact =>
                    {
                        CreatePersonResult person = await faceServiceClient.CreatePersonAsync(
                            // Id of the PersonGroup that the person belonged to
                            this.PersonGroupId,
                            contact.FullName);
                        using (Stream stream = new MemoryStream(contact.ImageBytes))
                        {
                            AddPersistedFaceResult persistedFaces = await faceServiceClient.AddPersonFaceAsync(PersonGroupId, person.PersonId, stream);
                            Persons.Add(new Person()
                            {
                                Name = contact.FullName,
                                PersonId = person.PersonId,
                                PersistedFaceIds = new Guid[] { persistedFaces.PersistedFaceId }
                            });
                        }

                    });
                    await faceServiceClient.TrainPersonGroupAsync(this.PersonGroupId);
                    TrainingStatus trainingStatus = null;
                    while (true)
                    {
                        try
                        {
                            trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(this.PersonGroupId);

                            if (trainingStatus.Status != Status.Running)
                            {
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                        await Task.Delay(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task<bool> CheckIfGroupExists()
        {
            // Test whether the group already exists
            bool groupExists = false;
            try
            {
                using (var faceServiceClient = new FaceServiceClient(this.strSubscriptionKey, this.strEndpoint))
                {

                    await faceServiceClient.GetPersonGroupAsync(this.PersonGroupId);
                    groupExists = true;
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

        private async void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            // Show file picker
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image files(*.jpg, *.png, *.bmp, *.gif) | *.jpg; *.png; *.bmp; *.gif";
            var result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                btnIdentify.IsEnabled = true;
                string filePath = dlg.FileName;
                Uri fileUri = new Uri(filePath);
                BitmapImage bitmapSource = new BitmapImage();
                bitmapSource.BeginInit();
                bitmapSource.CacheOption = BitmapCacheOption.None;
                bitmapSource.UriSource = fileUri;
                bitmapSource.EndInit();
                imgPhoto.Source = bitmapSource;

                // User picked one image
                // Clear previous detection and identification results
                TargetFaces.Clear();
                var pickedImagePath = dlg.FileName;
                var renderingImage = UIHelper.LoadImageAppliedOrientation(pickedImagePath);
                var imageInfo = UIHelper.GetImageInfoForRendering(renderingImage);
                SelectedFile = renderingImage;
                var faceServiceClient = new FaceServiceClient(this.strSubscriptionKey, this.strEndpoint);

                // Call detection REST API
                using (var fStream = File.OpenRead(pickedImagePath))
                {
                    try
                    {
                        var faces = await faceServiceClient.DetectAsync(fStream);
                        DrawingVisual visual = new DrawingVisual();
                        DrawingContext drawingContext = visual.RenderOpen();
                        drawingContext.DrawImage(bitmapSource,
                        new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
                        double dpi = bitmapSource.DpiX;
                        double resizeFactor = 96 / dpi;
                        // Convert detection result into UI binding object for rendering
                        foreach (var face in UIHelper.CalculateFaceRectangleForRendering(faces, MaxImageSize, imageInfo))
                        {
                            TargetFaces.Add(face);
                            drawingContext.DrawRectangle(
                            Brushes.Transparent,
                            new Pen(Brushes.Red, 2),
                                new Rect(
                                    face.FaceRectangle.Left * resizeFactor,
                                    face.FaceRectangle.Top * resizeFactor,
                                    face.FaceRectangle.Width * resizeFactor,
                                    face.FaceRectangle.Height * resizeFactor
                                )
                            );
                            drawingContext.Close();
                            RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
                            (int)(bitmapSource.PixelWidth * resizeFactor),
                            (int)(bitmapSource.PixelHeight * resizeFactor),
                            96,
                            96,
                            PixelFormats.Pbgra32);
                            faceWithRectBitmap.Render(visual);
                            imgPhoto.Source = faceWithRectBitmap;
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

        private void Photo_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                // If the REST call has not completed, return from this method.
                if (_faces == null)
                    return;
                var faces = _faces.ToArray();
                // Find the mouse position relative to the image.
                Point mouseXY = e.GetPosition(imgPhoto);
                ImageSource imageSource = imgPhoto.Source;
                BitmapSource bitmapSource = (BitmapSource)imageSource;
                double dpi = bitmapSource.DpiX;
                double resizeFactor = 96 / dpi;
                // Scale adjustment between the actual size and displayed size.
                var scale = imgPhoto.ActualWidth / (bitmapSource.PixelWidth / resizeFactor);

                for (int i = 0; i < faces.Length; ++i)
                {
                    FaceRectangle fr = faces[i].FaceRectangle;
                    double left = fr.Left * scale;
                    double top = fr.Top * scale;
                    double width = fr.Width * scale;
                    double height = fr.Height * scale;

                    // Display the face description for this face if the mouse is over this face rectangle.
                    if (mouseXY.X >= left && mouseXY.X <= left + width && mouseXY.Y >= top && mouseXY.Y <= top + height)
                    {
                        Contact contact = this.IdentifiedContacts.Where(con => faces[i].FaceId == con.FaceId).FirstOrDefault();
                        if (contact != null)
                        {
                            string strContactUrl = CrmHelper._organisationUrl + $"main.aspx?etn=contact&pagetype=entityrecord&id={contact.Id}";
                            Process.Start("chrome.exe", "strContactUrl");
                        }

                        break;
                    }
                }
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
                    var identifyResult = await faceServiceClient.IdentifyAsync(TargetFaces.Select(ff => ff.FaceId).ToArray(), personGroupId: this.PersonGroupId);
                    this.IdentifiedContacts = new List<Contact>();
                    var faces = TargetFaces.ToArray();
                    for (int idx = 0; idx < faces.Length; idx++)
                    {
                        // Update identification result for rendering
                        var face = TargetFaces[idx];
                        var res = identifyResult[idx];

                        string contactName;
                        if (res.Candidates.Length > 0 && !Persons.Any(p => p.PersonId == res.Candidates[0].PersonId))
                        {
                            contactName = Persons.Where(p => p.PersonId == res.Candidates[0].PersonId).First().Name;
                            IdentifiedContacts.Add(new Contact()
                            {
                                FullName = contactName,
                                Id = res.Candidates[0].PersonId.ToString(),
                                FaceId = face.FaceId

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
    }

    public class Contact
    {
        public string FullName { get; set; }
        public byte[] ImageBytes { get; set; }
        public string Id { get; set; }
        public Guid FaceId { get; set; }

    }

}
