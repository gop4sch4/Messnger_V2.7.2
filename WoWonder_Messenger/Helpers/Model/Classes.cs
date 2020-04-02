namespace WoWonder.Helpers.Model
{
    public class Classes
    {
        public class Categories
        {
            public string CategoriesId { get; set; }
            public string CategoriesName { get; set; }
            public string CategoriesColor { get; set; }
        }

        public class CallUser
        {
            public string VideoCall { get; set; }

            public string UserId { get; set; }
            public string Avatar { get; set; }
            public string Name { get; set; }

            //Data
            public string Id { get; set; } // call_id
            public string AccessToken { get; set; }
            public string AccessToken2 { get; set; }
            public string FromId { get; set; }
            public string ToId { get; set; }
            public string Active { get; set; }
            public string Called { get; set; }
            public string Time { get; set; }
            public string Declined { get; set; }
            public string Url { get; set; }
            public string Status { get; set; }
            public string RoomName { get; set; }
            public string Type { get; set; }

            //Style       
            public string TypeIcon { get; set; }
            public string TypeColor { get; set; }
        }


        public class SharedFile
        {
            public string FileName { set; get; }
            public string FileType { set; get; }
            public string FileDate { set; get; }
            public string FilePath { set; get; }
            public string FileExtension { set; get; }
            public string ImageExtra { set; get; }

        }

        public class Gender
        {
            public string GenderId { get; set; }
            public string GenderName { get; set; }
            public string GenderColor { get; set; }
            public bool GenderSelect { get; set; }
        }
    }

    public enum SystemApiGetLastChat
    {
        Old,
        New
    }
}