//
// System.Web.UI.TemplateControl.cs
//
// Authors:
// 	Duncan Mak  (duncan@ximian.com)
// 	Gonzalo Paniagua Javier (gonzalo@ximian.com)
//
// (C) 2002 Ximian, Inc. (http://www.ximian.com)
//

using System;
using System.Web.Compilation;
using System.Web.Util;

namespace System.Web.UI {

	public abstract class TemplateControl : Control, INamingContainer
	{
		static object abortTransaction = new object ();
		static object commitTransaction = new object ();
		static object error = new object ();

		#region Constructor
		protected TemplateControl ()
		{
			Construct ();
		}

		#endregion

		#region Properties

		protected virtual int AutoHandlers {
			get { return 0; }
			set { }
		}

		protected virtual bool SupportAutoEvents {
			get { return true; }
		}

		#endregion

		#region Methods

		protected virtual void Construct ()
		{
		}

		[MonoTODO]
		protected virtual LiteralControl CreateResourceBasedLiteralControl (int offset,
										    int size,
										    bool fAsciiOnly)
		{
			return null;
		}

		protected virtual void FrameworkInitialize ()
		{
		}

		Type GetTypeFromControlPath (string virtualPath)
		{
			if (virtualPath == null)
				throw new ArgumentNullException ("virtualPath");

			if (virtualPath [0] == '/')
				throw new ArgumentException ("Path cannot be rooted", "virtualPath");

			virtualPath = PathUtil.Combine (TemplateSourceDirectory, virtualPath);

			return UserControlCompiler.CompileUserControlType (new UserControlParser (virtualPath));
		}

		public Control LoadControl (string virtualPath)
		{
			object control = Activator.CreateInstance (GetTypeFromControlPath (virtualPath));
			if (control is UserControl)
				((UserControl) control).InitializeAsUserControl (Page);

			return (Control) control;
		}

		public ITemplate LoadTemplate (string virtualPath)
		{
			Type t = GetTypeFromControlPath (virtualPath);
			return new SimpleTemplate (t);
		}

		protected virtual void OnAbortTransaction (EventArgs e)
		{
			EventHandler eh = Events [error] as EventHandler;
			if (eh != null)
				eh.Invoke (this, e);
		}

		protected virtual void OnCommitTransaction (EventArgs e)
		{
			EventHandler eh = Events [commitTransaction] as EventHandler;
			if (eh != null)
				eh.Invoke (this, e);
		}

		protected virtual void OnError (EventArgs e)
		{
			EventHandler eh = Events [abortTransaction] as EventHandler;
			if (eh != null)
				eh.Invoke (this, e);
		}

		[MonoTODO]
		public Control ParseControl (string content)
		{
			return null;
		}

		[MonoTODO]
		public static object ReadStringResource (Type t)
		{
			return null;
		}

		[MonoTODO]
		protected void SetStringResourcePointer (object stringResourcePointer,
							 int maxResourceOffset)
		{
		}

		[MonoTODO]
		protected void WriteUTF8ResourceString (HtmlTextWriter output, int offset,
							int size, bool fAsciiOnly)
		{
		}

		#endregion

		#region Events

		public event EventHandler AbortTransaction {
			add { Events.AddHandler (abortTransaction, value); }
			remove { Events.RemoveHandler (abortTransaction, value); }
		}

		public event EventHandler CommitTransaction {
			add { Events.AddHandler (commitTransaction, value); }
			remove { Events.RemoveHandler (commitTransaction, value); }
		}

		public event EventHandler Error {
			add { Events.AddHandler (error, value); }
			remove { Events.RemoveHandler (error, value); }
		}

		#endregion

		class SimpleTemplate : ITemplate
		{
			Type type;

			public SimpleTemplate (Type type)
			{
				this.type = type;
			}

			public void InstantiateIn (Control control)
			{
				object template = Activator.CreateInstance (type);
				control.Controls.Add ((Control) template);
			}
		}
	}
}
