﻿#pragma checksum "..\..\..\UserControl\FaceIdentify.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "219A99DCFEAE419DA2F3FFF34D63413C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Hsl.CognitiveServices.Demo.UserControl;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Hsl.CognitiveServices.Demo.UserControl {
    
    
    /// <summary>
    /// FaceIdentify
    /// </summary>
    public partial class FaceIdentify : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Hsl.CognitiveServices.Demo.UserControl.FaceIdentify FaceIdentificationPane;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnClose;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridFaceAPIKeyManagement;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtSubscriptionKey;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtEndpoint;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSaveFaceSubScription;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnNextFaceSubScription;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridTrainPersonGroup;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnTrain;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnTrainSkip;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar PBTrainFaceApi;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridIdentifyPerson;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image ImageDisplay;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnIdentify;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\..\UserControl\FaceIdentify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar PBIdentify;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Hsl.CognitiveServices.Demo;component/usercontrol/faceidentify.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UserControl\FaceIdentify.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.FaceIdentificationPane = ((Hsl.CognitiveServices.Demo.UserControl.FaceIdentify)(target));
            return;
            case 2:
            this.btnClose = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\..\UserControl\FaceIdentify.xaml"
            this.btnClose.Click += new System.Windows.RoutedEventHandler(this.BtnClose_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.gridFaceAPIKeyManagement = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.txtSubscriptionKey = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.txtEndpoint = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.btnSaveFaceSubScription = ((System.Windows.Controls.Button)(target));
            return;
            case 7:
            this.btnNextFaceSubScription = ((System.Windows.Controls.Button)(target));
            
            #line 41 "..\..\..\UserControl\FaceIdentify.xaml"
            this.btnNextFaceSubScription.Click += new System.Windows.RoutedEventHandler(this.BtnNextKeyManagement_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.gridTrainPersonGroup = ((System.Windows.Controls.Grid)(target));
            return;
            case 9:
            this.btnTrain = ((System.Windows.Controls.Button)(target));
            
            #line 59 "..\..\..\UserControl\FaceIdentify.xaml"
            this.btnTrain.Click += new System.Windows.RoutedEventHandler(this.BtnTrain_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.btnTrainSkip = ((System.Windows.Controls.Button)(target));
            
            #line 60 "..\..\..\UserControl\FaceIdentify.xaml"
            this.btnTrainSkip.Click += new System.Windows.RoutedEventHandler(this.BtnTrainSkip_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.PBTrainFaceApi = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 12:
            this.gridIdentifyPerson = ((System.Windows.Controls.Grid)(target));
            
            #line 67 "..\..\..\UserControl\FaceIdentify.xaml"
            this.gridIdentifyPerson.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.gridIdentifyPerson_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 13:
            this.ImageDisplay = ((System.Windows.Controls.Image)(target));
            return;
            case 14:
            
            #line 100 "..\..\..\UserControl\FaceIdentify.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.UploadImage_Click);
            
            #line default
            #line hidden
            return;
            case 15:
            this.btnIdentify = ((System.Windows.Controls.Button)(target));
            
            #line 101 "..\..\..\UserControl\FaceIdentify.xaml"
            this.btnIdentify.Click += new System.Windows.RoutedEventHandler(this.BtnIdentify_Click);
            
            #line default
            #line hidden
            return;
            case 16:
            this.PBIdentify = ((System.Windows.Controls.ProgressBar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

