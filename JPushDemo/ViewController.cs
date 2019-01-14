using CoreLocation;
using Foundation;
using System;
using System.Linq;
using UIKit;

namespace JPushDemo
{
    public partial class ViewController : UIViewController
    {
		public ViewController (IntPtr handle) : base (handle)
		{
		}


        public CLLocationManager location;
        public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
            // Perform any additional setup after loading the view, typically from a nib.

            location = new CLLocationManager();
            location.RequestAlwaysAuthorization();
            //location.StartUpdatingLocation();
            location.PausesLocationUpdatesAutomatically = false;
            location.AllowsBackgroundLocationUpdates = true;
            location.DesiredAccuracy = CLLocation.AccuracyHundredMeters;
            location.Delegate = new MyLocationDelegate(location);
        }

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

        partial void LocationBtnClick(UIKit.UIButton sender)
        {
            location.RequestLocation();
        }
    }
    public class MyLocationDelegate : CLLocationManagerDelegate
    {
        CLLocationManager locationManager;
        public MyLocationDelegate(CLLocationManager locationManager)
        {
            this.locationManager = locationManager;
        }
        public override void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
        {
            CLLocation location = locations.FirstOrDefault();

            var latitude = location.Coordinate.Latitude;
            var longitude = location.Coordinate.Longitude;
            Console.WriteLine("-----------" + latitude + "Longitude" + longitude);

            //locationManager.StopUpdatingLocation();
        }

        public override void Failed(CLLocationManager manager, NSError error)
        {
            Console.WriteLine(error);
        }
    }
}