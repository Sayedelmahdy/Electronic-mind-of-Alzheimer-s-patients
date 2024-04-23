namespace BLL.DTOs.PatientDto
{
    public class PersonInImage
    {
        public string FamilyName { get; set; }
        public string RelationalityOfThisPatient { get; set; }
        public double? FamilyLatitude { get; set; }
        public double? FamilyLongitude { get; set; }
        public string FamilyPhoneNumber { get; set; }
        public string DescriptionForPatient { get; set; }
        public string FamilyAvatarUrl { get; set; }
    }
}