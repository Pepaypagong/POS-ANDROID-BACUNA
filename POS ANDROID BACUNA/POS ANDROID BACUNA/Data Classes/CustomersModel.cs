using SQLite;

namespace POS_ANDROID_BACUNA
{
    public class CustomersModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }
        public string FullName { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
    }
}