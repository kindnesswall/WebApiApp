using KindnessWall.Helper;

namespace KindnessWall.Dto
{
    public class BaseDto
    {
        public string Id { get; set; }

        public string CreateDateTime { get; set; }

        public string CreateDate => CreateDateTime.GetDateFromShamsiDateTime();

        public string CreateTime => CreateDateTime.GetTimeFromShamsiDateTime();
    }
}