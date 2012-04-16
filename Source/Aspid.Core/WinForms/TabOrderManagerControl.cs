#region License
#endregion
//Based on code by Scott McMaster http://www.codeproject.com/KB/dotnet/TabOrderManager.aspx

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
//using System.Drawing;

namespace Aspid.Core.WinForms
{
    [ProvideProperty("TabControlComparerType", typeof(Control))]
	[Description("Wrap the TabOrderManager class and supply extendee controls with a custom tab scheme" )]
    //[ToolboxBitmap( typeof(TabSchemeProvider), "TabSchemeProvider" )]
    public class TabOrderManagerControl : Component, IExtenderProvider
    {
        private static readonly ILogger logger = Logging.GetLogger(typeof(TabOrderManagerControl));

        public enum TabControlComparerType
        {
            None,
            AcrossFirst,
            DownFirst,
        }

        /// <summary>
        /// HashSet to store the controls that use our extender property.
		/// </summary>
        private Dictionary<Control, TabControlComparerType> _extendees = new Dictionary<Control, TabControlComparerType>();

		/// <summary>
		/// The form we're hosted on, which will be calculated by watching the extendees entering the control hierarchy.
		/// </summary>
		private Form _topLevelForm;
        
        public TabOrderManagerControl()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
		}
        
		/// <summary>
		/// Hook up to the form load event and indicate that we've done so.
		/// </summary>
		private void HookFormLoad()
		{
			if( _topLevelForm == null )  return;
            _topLevelForm.Load += TopLevelForm_Load;
		}

		/// <summary>
		/// Unhook from the form load event and indicate that we need to do so again before applying tab schemes.
		/// </summary>
		private void UnhookFormLoad()
		{
			if( _topLevelForm == null ) return;
            _topLevelForm.Load -= TopLevelForm_Load;
			_topLevelForm = null;
		}

		/// <summary>
		/// Hook up to all of the parent changed events for this control and its ancestors so that we are informed
		/// if and when they are added to the top-level form (whose load event we need).
		/// It's not adequate to look at just the control, because it may have been added to its parent, but the parent
		/// may not be descendent of the form -yet-.
		/// </summary>
		/// <param name="c"></param>
		private void HookParentChangedEvents(Control control)
		{
			while(control != null)
			{
                //(Make sure not to duplicate the handler)
                control.ParentChanged -= Extendee_ParentChanged;
				control.ParentChanged += Extendee_ParentChanged;
				control = control.Parent;
			}
		}
                
        /// <summary>
        /// Get whether or not we're managing a given control.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [DefaultValue(TabControlComparerType.None)]
        public TabControlComparerType GetTabControlComparerType(Control control)
        {
            if (!_extendees.ContainsKey(control)) return TabControlComparerType.None;
            return _extendees[control];
        }

		/// <summary>
		/// Set the tab scheme to use on a given control
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
        public void SetTabControlComparerType(Control control, TabControlComparerType commparerType)
        {
            if (commparerType == TabControlComparerType.None)
            {
                _extendees.Remove(control);

                // If we no longer have any extendees, we don't need to be wired up to the form load event.
                if (_extendees.Count == 0)
                {
                    UnhookFormLoad();
                }

                return;
            }
            
            _extendees[control] = commparerType;
            if (_topLevelForm != null) return;
                        
            if (control.TopLevelControl != null)
            {
                // We're in luck.
                // This is the form, or this control knows about it, so take the opportunity to grab it and wire up to its Load event.
                _topLevelForm = control.TopLevelControl as Form;
                HookFormLoad();
            }
            else
            {
                // Set up to wait around until this control or one of its ancestors is added to the form's control hierarchy.
                HookParentChangedEvents(control);
            }
        }
        	
		public bool CanExtend(object extendee)
		{
			return (extendee is Form || extendee is Panel || extendee is GroupBox || extendee is UserControl || extendee is ScrollableControl);
		}
        
		private void TopLevelForm_Load(object sender, EventArgs e)
		{
			var form = sender as Form;
            if (form == null) return;

			var tabOrderManager = new TabOrderManager(form);
            
            TabControlComparer comparer = TabControlComparer.None;
            foreach (var extendee in _extendees)
            {
                var control = extendee.Key;
                
                if (control == form)
                {
                    comparer = GetTabControlComparerFromType(extendee.Value);
                }
                else
                {
                    tabOrderManager.SetTabControlComparer(control, GetTabControlComparerFromType(extendee.Value));
                }
            }

            tabOrderManager.SetTabOrder(comparer);
		}

        private static TabControlComparer GetTabControlComparerFromType(TabControlComparerType tabControlComparerType)
        {
            switch (tabControlComparerType)
            {
                case TabControlComparerType.None:
                    return TabControlComparer.None;
                case TabControlComparerType.AcrossFirst:
                    return TabControlComparer.AcrossFirst;
                case TabControlComparerType.DownFirst:
                    return TabControlComparer.DownFirst;
                default:
                    return TabControlComparer.None;
            }
        }

		/// <summary>
		/// We track when each extendee's parent is changed, and also when their parents are changed, until
		/// SOMEBODY finally changes their parent to the form, at which point we can hook the load to apply
		/// the tab schemes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Extendee_ParentChanged(object sender, EventArgs e)
		{
            // We've already found the form and attached a load event handler, so there's nothing left to do.
			if(_topLevelForm != null) return;
			
			var control = sender as Control;
            if (control.TopLevelControl != null && control.TopLevelControl is Form)
            {
                // We found the form, so we're done.
                _topLevelForm = control.TopLevelControl as Form;
                HookFormLoad();
                UnhookParentChangedEvents(_topLevelForm);
            }
            else
            {
                //The new parent doesn't know about the top level form yet, so hook to the parent changed events
                HookParentChangedEvents(control);
            }
		}

        private void UnhookParentChangedEvents(Control control)
        {
            if (control == null) return;

            control.ParentChanged -= Extendee_ParentChanged;
            foreach (Control childControl in control.Controls)
            {
                UnhookParentChangedEvents(childControl);
            }
        }
    }
}
