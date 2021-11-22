package crc6448954c09801b2d98;


public class BuscadorSelectorFragment
	extends android.support.v4.app.DialogFragment
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onCreateView:(Landroid/view/LayoutInflater;Landroid/view/ViewGroup;Landroid/os/Bundle;)Landroid/view/View;:GetOnCreateView_Landroid_view_LayoutInflater_Landroid_view_ViewGroup_Landroid_os_Bundle_Handler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"";
		mono.android.Runtime.register ("APP.Fragments.BuscadorSelectorFragment, APP", BuscadorSelectorFragment.class, __md_methods);
	}


	public BuscadorSelectorFragment ()
	{
		super ();
		if (getClass () == BuscadorSelectorFragment.class)
			mono.android.TypeManager.Activate ("APP.Fragments.BuscadorSelectorFragment, APP", "", this, new java.lang.Object[] {  });
	}

	public BuscadorSelectorFragment (java.lang.String p0)
	{
		super ();
		if (getClass () == BuscadorSelectorFragment.class)
			mono.android.TypeManager.Activate ("APP.Fragments.BuscadorSelectorFragment, APP", "System.String, mscorlib", this, new java.lang.Object[] { p0 });
	}

	public BuscadorSelectorFragment (java.lang.String p0, int p1)
	{
		super ();
		if (getClass () == BuscadorSelectorFragment.class)
			mono.android.TypeManager.Activate ("APP.Fragments.BuscadorSelectorFragment, APP", "System.String, mscorlib:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1 });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public android.view.View onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2)
	{
		return n_onCreateView (p0, p1, p2);
	}

	private native android.view.View n_onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2);


	public void onResume ()
	{
		n_onResume ();
	}

	private native void n_onResume ();

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
