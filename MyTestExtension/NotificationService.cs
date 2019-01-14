using System;
using Foundation;
using UIKit;
using UserNotifications;

namespace MyTestExtension
{
    [Register("NotificationService")]
    public class NotificationService : UNNotificationServiceExtension
    {
        Action<UNNotificationContent> ContentHandler { get; set; }
        UNMutableNotificationContent BestAttemptContent { get; set; }

        protected NotificationService(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
        {
            ContentHandler = contentHandler;
            BestAttemptContent = (UNMutableNotificationContent)request.Content.MutableCopy();

            // Modify the notification content here...
            BestAttemptContent.Title = string.Format("{0}[modified]", BestAttemptContent.Title);

            ContentHandler(BestAttemptContent);


            ContentHandler = contentHandler;
            BestAttemptContent = (UNMutableNotificationContent)request.Content.MutableCopy();
            // Modify the notification content here...
            //self.bestAttemptContent.title = [NSString stringWithFormat: @"%@ [modified]", self.bestAttemptContent.title];

            //// 重写一些东西
            //self.bestAttemptContent.title = @"我是标题";
            //self.bestAttemptContent.subtitle = @"我是子标题";
            //self.bestAttemptContent.body = @"来自徐不同";

            // 附件
            NSDictionary dict = BestAttemptContent.UserInfo;
            NSDictionary notiDict = dict["aps"] as NSDictionary;
            NSString imgUrl = notiDict["imageAbsoluteString"] as NSString;

            if (imgUrl.Length == null)
            {
                ContentHandler(BestAttemptContent);
            }
            //        [self loadAttachmentForUrlString:imgUrl withType:@"png" completionHandle:^(UNNotificationAttachment* attach) {

            //    if (attach) {
            //        self.bestAttemptContent.attachments = [NSArray arrayWithObject:attach];
            //    }
            //self.contentHandler(self.bestAttemptContent);

            //}];
            loadAttachmentForUrlString(imgUrl, "png", (attach) =>
            {
                if (attach != null)
                {
                    BestAttemptContent.Attachments = new UNNotificationAttachment[] { attach };//[NSArray arrayWithObject: attach];
                }
                ContentHandler(BestAttemptContent);
            });
        }

        delegate void CompletionHandler(UNNotificationAttachment attach);
        void loadAttachmentForUrlString(string urlString, string type, CompletionHandler completionHandler)
        {
            NSUrl attachmentURL = new NSUrl(urlString);
            //NSString fileExt = [self fileExtensionForMediaType: type];

            NSUrlSession session = NSUrlSession.FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
            session.CreateDownloadTask(attachmentURL, (url, response, error) => {

                if (error != null)
                {

                }
                else
                {
                    NSFileManager fileManager = NSFileManager.DefaultManager;
                    NSUrl localUrl = new NSUrl(url.Path, false);
                    NSError error1 = null;
                    fileManager.Move(url, localUrl, out error1);

                    NSError attachmentError = null;
                    UNNotificationAttachment attachment = UNNotificationAttachment.FromIdentifier("", localUrl, options: null, error: out attachmentError);
                    if (attachmentError != null)
                    {

                    }
                    else
                    {
                        completionHandler(attachment);
                    }
                }
            }).Resume();

//    [[session downloadTaskWithURL:attachmentURL
//                completionHandler:^(NSURL* temporaryFileLocation, NSURLResponse* response, NSError* error) {
//                    if (error != nil) {
//                        NSLog(@"%@", error.localizedDescription);
//    } else {
//                        NSFileManager* fileManager = [NSFileManager defaultManager];
//    NSURL* localURL = [NSURL fileURLWithPath:[temporaryFileLocation.path stringByAppendingString: fileExt]];
//    [fileManager moveItemAtURL:temporaryFileLocation toURL:localURL error:&error];
                        
//                        NSError* attachmentError = nil;
//    attachment = [UNNotificationAttachment attachmentWithIdentifier:@"" URL:localURL options:nil error:&attachmentError];
//                        if (attachmentError) {
//                            NSLog(@"%@", attachmentError.localizedDescription);
//}
//                    }
//                     completionHandler(attachment);
//               }] resume];
        }

        public override void TimeWillExpire()
        {
            // Called just before the extension will be terminated by the system.
            // Use this as an opportunity to deliver your "best attempt" at modified content, otherwise the original push payload will be used.

            ContentHandler(BestAttemptContent);
        }
    }
}
