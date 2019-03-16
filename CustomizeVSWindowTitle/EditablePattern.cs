using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace ErwinMayerLabs.CustomizeVSWindowTitleExtension
{
    public class PreviewRequiresAttribute : Attribute
    {
        public enum Requirement {
            None,
            Solution,
            Document,
        }
        public readonly Requirement Require;
        public PreviewRequiresAttribute(Requirement requires) : base() { Require=requires; }
    }

    public class EditablePatternEditor : UITypeEditor
    {
        IWindowsFormsEditorService editorService;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                editorService =
                    provider.GetService(
                    typeof(IWindowsFormsEditorService))
                    as IWindowsFormsEditorService;
            }

            object page = context.Instance;

            PropertyInfo fi = page.GetType().GetProperty(context.PropertyDescriptor.Name);
            string ep = fi.GetValue(page, new object[] { }) as string; // get the attribute value
            string defVal = null;
            {
                var attr = fi.GetCustomAttributes(typeof(DefaultValueAttribute), false);
                if (attr!=null && attr.Length>0)
                {
                    defVal= ((DefaultValueAttribute)attr[0]).Value.ToString();
                }
            }

            PreviewRequiresAttribute.Requirement req = PreviewRequiresAttribute.Requirement.None;
            {
                var attr = fi.GetCustomAttributes(typeof(PreviewRequiresAttribute), false);
                if (attr!=null && attr.Length>0)
                {
                    req= ((PreviewRequiresAttribute)attr[0]).Require;
                }
            }

            if (editorService != null)
            {

                EditablePatternControl ctl = new EditablePatternControl(editorService);
                ctl.Pattern=ep;
                ctl.PreviewRequires=req;
                ctl.DefaultPattern=defVal;
                ctl.BorderStyle= BorderStyle.None;
                editorService.DropDownControl(ctl);

                ep=ctl.Pattern;
            }

            return ep;
        }
    }
}
