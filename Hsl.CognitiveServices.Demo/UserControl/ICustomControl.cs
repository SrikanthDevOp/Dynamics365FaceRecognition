using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hsl.CognitiveServices.Demo
{
    public delegate void Close(ICustomControl sender, EventArgs e);
    public interface ICustomControl
    {
        /// <summary>
        /// This event will be fired from user control and will be listened
        /// by parent form. when this event will be fired from child control
        /// parent will close the form
        /// </summary>
        event Close CloseInitiated;
        /// <summary>
        /// This is unique name for the control. This will be used in dictonary object
        /// to keep track of the opened user control in parent form.
        /// </summary>
    }
}
