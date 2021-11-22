package mono.com.github.aakira.expandablelayout;


public class ExpandableLayoutListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.github.aakira.expandablelayout.ExpandableLayoutListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onAnimationEnd:()V:GetOnAnimationEndHandler:Com.Github.Aakira.Expandablelayout.IExpandableLayoutListenerInvoker, Naxam.ExpandableLayout.Droid\n" +
			"n_onAnimationStart:()V:GetOnAnimationStartHandler:Com.Github.Aakira.Expandablelayout.IExpandableLayoutListenerInvoker, Naxam.ExpandableLayout.Droid\n" +
			"n_onClosed:()V:GetOnClosedHandler:Com.Github.Aakira.Expandablelayout.IExpandableLayoutListenerInvoker, Naxam.ExpandableLayout.Droid\n" +
			"n_onOpened:()V:GetOnOpenedHandler:Com.Github.Aakira.Expandablelayout.IExpandableLayoutListenerInvoker, Naxam.ExpandableLayout.Droid\n" +
			"n_onPreClose:()V:GetOnPreCloseHandler:Com.Github.Aakira.Expandablelayout.IExpandableLayoutListenerInvoker, Naxam.ExpandableLayout.Droid\n" +
			"n_onPreOpen:()V:GetOnPreOpenHandler:Com.Github.Aakira.Expandablelayout.IExpandableLayoutListenerInvoker, Naxam.ExpandableLayout.Droid\n" +
			"";
		mono.android.Runtime.register ("Com.Github.Aakira.Expandablelayout.IExpandableLayoutListenerImplementor, Naxam.ExpandableLayout.Droid", ExpandableLayoutListenerImplementor.class, __md_methods);
	}


	public ExpandableLayoutListenerImplementor ()
	{
		super ();
		if (getClass () == ExpandableLayoutListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Github.Aakira.Expandablelayout.IExpandableLayoutListenerImplementor, Naxam.ExpandableLayout.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onAnimationEnd ()
	{
		n_onAnimationEnd ();
	}

	private native void n_onAnimationEnd ();


	public void onAnimationStart ()
	{
		n_onAnimationStart ();
	}

	private native void n_onAnimationStart ();


	public void onClosed ()
	{
		n_onClosed ();
	}

	private native void n_onClosed ();


	public void onOpened ()
	{
		n_onOpened ();
	}

	private native void n_onOpened ();


	public void onPreClose ()
	{
		n_onPreClose ();
	}

	private native void n_onPreClose ();


	public void onPreOpen ()
	{
		n_onPreOpen ();
	}

	private native void n_onPreOpen ();

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
