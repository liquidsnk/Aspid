#region License
#endregion
//Based on code by Scott McMaster http://www.codeproject.com/KB/dotnet/TabOrderManager.aspx

using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Aspid.Core.Extensions;

namespace Aspid.Core.WinForms
{
    public class TabOrderManager
    {
        private static readonly ILogger logger = Logging.GetLogger(typeof(TabOrderManager));

		private Control _mainContainer;
        private TabControlComparer _tabControlComparer = TabControlComparer.None;
        
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="container">The container whose tab order we manage.</param>
		public TabOrderManager(Control container)
		{
            container.ThrowIfNull("container");

			_mainContainer = container;
		}

        public void SetTabControlComparer(TabControlComparer tabControlComparer)
        {
            tabControlComparer.ThrowIfNull("tabControlComparer");

            _tabControlComparer = tabControlComparer;
        }

        private Dictionary<Control, TabControlComparer> _controlOverrides = new Dictionary<Control, TabControlComparer>();

        public void SetTabControlComparer(Control control, TabControlComparer tabControlComparer)
        {
            control.ThrowIfNull("control");
            tabControlComparer.ThrowIfNull("tabControlComparer");

            _controlOverrides[control] = tabControlComparer;
        }
        
        /// <summary>
        /// Recursively set the tab order on this container and all of its children.
        /// </summary>
        public void SetTabOrder()
        {
            SetTabOrder(_tabControlComparer);
        }

		/// <summary>
		/// Recursively set the tab order on this container and all of its children.
		/// </summary>
		/// <param name="scheme">The tab ordering strategy to apply.</param>
		public void SetTabOrder(TabControlComparer tabControlComparer)
		{
            SetTabOrder(tabControlComparer, 0, _mainContainer);
        }
        
        private int SetTabOrder(TabControlComparer tabControlComparer, int firstTabIndex, Control container)
		{
			try
			{
                if (container.Controls == null || container.Controls.Count == 0) return firstTabIndex;

                int currentTabIndex = firstTabIndex;
				var controls = new List<Control>();
                controls.AddRange(container.Controls.OfType<Control>());
				controls.Sort(tabControlComparer);

				foreach(Control control in controls)
				{
                    logger.LogDebug("TabOrderManager:  Changing tab index for " + control.Name);
					
					control.TabIndex = currentTabIndex++;
					if(control.Controls.Count > 0)
					{
						// Control has children -- recurse.
                        var comparer = tabControlComparer;
                        if (_controlOverrides.ContainsKey(control))
                        {
                            comparer = _controlOverrides[control];
                        }

                        currentTabIndex = SetTabOrder(comparer, currentTabIndex, control);
					}
				}

				return currentTabIndex;
			}
			catch(Exception e)
			{
                logger.LogError("Exception in TabOrderManager.SetTabOrder");
                logger.LogException(e);
				return 0;
			}
        }
    }
}
