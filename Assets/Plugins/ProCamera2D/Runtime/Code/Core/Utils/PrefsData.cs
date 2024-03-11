using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public static class PrefsData
    {
    	// ProCamera2D
    	public static string NumericBoundariesColorKey = "Numeric Boundaries";
    	public static Color NumericBoundariesColorValue = Color.white;

    	public static string TargetsMidPointColorKey = "Targets Mid Point";
    	public static Color TargetsMidPointColorValue = Color.yellow;

    	public static string InfluencesColorKey = "Influences Sum";
    	public static Color InfluencesColorValue = Color.red;

    	public static string ShakeInfluenceColorKey = "Shake Influence";
    	public static Color ShakeInfluenceColorValue = Color.red;

    	public static string OverallOffsetColorKey = "Overall Offset";
    	public static Color OverallOffsetColorValue = Color.yellow;

    	public static string CamDistanceColorKey = "Camera Distance Limit";
    	public static Color CamDistanceColorValue = Color.red;

    	public static string CamTargetPositionColorKey = "Camera Target Position";
    	public static Color CamTargetPositionColorValue = new Color(.3f, .3f, .1f);

    	public static string CamTargetPositionSmoothedColorKey = "Camera Target Position Smoothed";
    	public static Color CamTargetPositionSmoothedColorValue = new Color(.5f, .3f, .1f);

    	public static string CurrentCameraPositionColorKey = "Current Camera Position";
    	public static Color CurrentCameraPositionColorValue = new Color(.8f, .3f, .1f);

    	public static string CameraWindowColorKey = "Camera Window";
    	public static Color CameraWindowColorValue = Color.red;

    	// Forward Focus
		public static string ForwardFocusColorKey = "Forward Focus";
    	public static Color ForwardFocusColorValue = Color.red; 

    	// Zoom To Fit
		public static string ZoomToFitColorKey = "Zoom To Fit";
    	public static Color ZoomToFitColorValue = Color.magenta;

        // Boundaries Trigger
        public static string BoundariesTriggerColorKey = "Trigger Boundaries";
        public static Color BoundariesTriggerColorValue = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, .3f);

        // Influence Trigger
        public static string InfluenceTriggerColorKey = "Trigger Influence";
        public static Color InfluenceTriggerColorValue = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, .3f);

        // Zoom Trigger
        public static string ZoomTriggerColorKey = "Trigger Zoom";
        public static Color ZoomTriggerColorValue = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, .3f);

        // Trigger shape
        public static string TriggerShapeColorKey = "Trigger Shape";
        public static Color TriggerShapeColorValue = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, .3f);

        // Rails
        public static string RailsColorKey = "Rails";
        public static Color RailsColorValue = Color.white;

        public static float RailsSnapping = .1f;

        // Pan Edges
        public static string PanEdgesColorKey = "Pan Edges";
        public static Color PanEdgesColorValue = Color.red;

        // Rooms
        public static string RoomsColorKey = "Rooms";
        public static Color RoomsColorValue = Color.red;

        public static float RoomsSnapping = .1f;
	    
	    // Content Fitter
	    public static string FitterFillColorKey = "Fitter Fill";
	    public static Color FitterFillColorValue = new Color(1f, 1f, 1f, 0.1f);
	    public static string FitterLineColorKey = "Fitter Line";
	    public static Color FitterLineColorValue = new Color(1f, 1f, 1f, 0.6f);
    }
}