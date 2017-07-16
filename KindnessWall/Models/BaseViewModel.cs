using KindnessWall.Helper;

namespace KindnessWall.Models
{
    public class BaseViewModel
    {
        public int Id { get; set; }

        public string CreateDateTime { get; set; }

        public string CreateDate => CreateDateTime.GetDateFromShamsiDateTime();

        public string CreateTime => CreateDateTime.GetTimeFromShamsiDateTime();
    }
}