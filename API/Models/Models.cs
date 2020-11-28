using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class ResponseMessage
    {
        public string Success;
        public string Message;
        public string InternalMessage;
    }

    [Serializable]
    public class User
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
    }

    //Member List Search Get
    [Serializable]
    public class MemberList
    {
        public int Mem_ID { get; set; }
        public string Cmp_Key { get; set; }
        public string Cmp_Name { get; set; }
        public string AdmissionDate { get; set; }
        public string GRNo { get; set; }
        public string RollNo { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Std { get; set; }
        public string Division { get; set; }
        public string DOB { get; set; }
        public string Logo { get; set; }
        public string Address { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string Mobile3 { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Photo { get; set; }
        public string Mobile1_Pass { get; set; }
        public string Mobile2_Pass { get; set; }
        public string Mobile3_Pass { get; set; }
        public string TokenID1 { get; set; }
        public string TokenID2 { get; set; }
        public string TokenID3 { get; set; }
        public string DisplayType { get; set; }
        public string DealerType { get; set; }
    }

    //Memebrs List Search 
    [Serializable]
    public class MemberLists
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public List<MemberList> MemberList { get; set; }
    }

    //UserLogin
    [Serializable]
    public class UserLogin
    {
        public string CmpID { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string TokenID { get; set; }
    }

    [Serializable]
    public class StringCompany
    {
        public string Cmp_ID { get; set; }
        public string Types { get; set; }
        public string Name { get; set; }
        public string DealerType { get; set; }
        public string DealerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Distict { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Pincode { get; set; }
        public string Owner { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string PageHeader { get; set; }
        public string Cmp_Key { get; set; }
        public string ExpiryDate { get; set; }
        public string Password { get; set; }
        public string GPSLocation { get; set; }
        public string DisplayType { get; set; }
        public string PageHeader1 { get; set; }
        public string PageHeader2 { get; set; }
        public string Facebook { get; set; }
        public string Youtube { get; set; }
        public string Instagram { get; set; }
        public string Google { get; set; }
        public string Twitter { get; set; }
        public string WhatsApp { get; set; }
        public string Class_Attendance { get; set; }
        public string Auto_Attendance { get; set; }
        public string Timetable { get; set; }
        public string Test { get; set; }
        public string Homework { get; set; }
        public string Classwork { get; set; }
        public string Notice { get; set; }
        public string News { get; set; }
        public string Exam { get; set; }
        public string Fees { get; set; }
        public string Leave { get; set; }
        public string Chat { get; set; }
        public string Video_Gallery { get; set; }
        public string Photo_Gallery { get; set; }
        public string PDF_Gallery { get; set; }
        public string Lecture_Attendance { get; set; }

    }

    [Serializable]
    public class StringCompanies
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public List<StringCompany> Company { get; set; }
    }

    //Studens Attendance
    [Serializable]
    public class StudAttendance
    {
        public string Date { get; set; }
        public string Lacture { get; set; }
        public string Faculty { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
    }

    //Student Attendnace Details
    [Serializable]
    public class StudAttendances
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public string Standard { get; set; }
        public string Division { get; set; }
        public string LeaveDay { get; set; }
        public string AbsentDay { get; set; }
        public string HolliDay { get; set; }
        public string PresentDay { get; set; }
        public string AttendnaceDay { get; set; }
        public List<StudAttendance> StudAttendance { get; set; }
    }

    [Serializable]
    public class StudAtt
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public string Standard { get; set; }
        public string Division { get; set; }
        public List<StudAttendance> StudAttendance { get; set; }
    }

    [Serializable]
    public class StudTest
    {
        public string Date { get; set; }
        public string Round { get; set; }
        public string Faculty { get; set; }
        public string Subject { get; set; }
        public string TotalMarks { get; set; }
        public string PassMarks { get; set; }
        public string Marks { get; set; }
        public string Status { get; set; }
    }

    [Serializable]
    public class StudTests
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public string Standard { get; set; }
        public string Division { get; set; }
        public List<StudTest> StudTest { get; set; }
    }

    [Serializable]
    public class StudTimetable
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string Subject { get; set; }
        public string Faculty { get; set; }
        public string Type { get; set; }
        public string Remarks { get; set; }
    }

    [Serializable]
    public class StudTimetables
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public string Standard { get; set; }
        public string Division { get; set; }
        public List<StudTimetable> StudTimetable { get; set; }
    }

    [Serializable]
    public class StudFee
    {
        public string Fees_Type { get; set; }
        public string Fees_Amount { get; set; }
        public string Fees_Date { get; set; }
        public string PaymentMode { get; set; }
        public string Remarks { get; set; }
    }

    [Serializable]
    public class StudFees
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public string Credit { get; set; }
        public string Debit { get; set; }
        public List<StudFee> StudFee { get; set; }
    }

    //Leave
    [Serializable]
    public class StudLeave
    {
        public string LeaveFrom { get; set; }
        public string LeaveTo { get; set; }
        public string LeaveReason { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
    }

    //Fee Details
    [Serializable]
    public class StudLeaves
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public List<StudLeave> StudLeave { get; set; }
    }

    [Serializable]
    public class PublicNotice
    {
        public string PI_ID { get; set; }
        public string Cmp_ID { get; set; }
        public string Date { get; set; }
        public string Advertise { get; set; }
        public string Message { get; set; }
        public string Photo { get; set; }
        public string Weblink { get; set; }
        public string Expiry_Date { get; set; }
        public string Status { get; set; }
    }

    [Serializable]
    public class PublicNotices
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public List<PublicNotice> PubNotices { get; set; }
    }

    //Notice Message
    [Serializable]
    public class StudNotice
    {
        public string Notice_Date { get; set; }
        public string Notice_Type { get; set; }
        public string Notice_Msg { get; set; }
        public string File1 { get; set; }
        public string File1Type { get; set; }
        public string File1Size { get; set; }
        public string File2 { get; set; }
        public string File2Type { get; set; }
        public string File2Size { get; set; }
        public string File3 { get; set; }
        public string File3Type { get; set; }
        public string File3Size { get; set; }
        public string Replay_Type { get; set; }
        public string Notice_Replay { get; set; }
        public string Notification { get; set; }
        public string Mobile1_Status { get; set; }
        public string Mobile2_Status { get; set; }
        public string Mobile3_Status { get; set; }
        public string ViewMessage { get; set; }
        public string CmpID { get; set; }
    }

    //Notice Messages
    [Serializable]
    public class StudNotices
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public List<StudNotice> StudNotice { get; set; }
    }

    [Serializable]
    public class StudChat
    {
        public string Chat_Date { get; set; }
        //public string Chat_Type { get; set; }
        public string Chat_Msg { get; set; }
        public string File1 { get; set; }
        public string File1Type { get; set; }
        //public string File1Size { get; set; }
        public string Replay_Type { get; set; }
        //public string Chat_Replay { get; set; }
        public string Notification { get; set; }
        //public string Mobile1_Status { get; set; }
        //public string Mobile2_Status { get; set; }
        // public string Mobile3_Status { get; set; }
        public string ViewMessage { get; set; }
        public string CmpID { get; set; }
    }

    //Chat Messages
    [Serializable]
    public class StudChats
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public List<StudChat> StudChat { get; set; }
    }

    [Serializable]
    public class PhotoGalleryItem
    {
        public string Cmp_ID { get; set; }
        public string Std { get; set; }
        public string Division { get; set; }
        public string Title { get; set; }
        public string Photo { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
    }

    [Serializable]
    public class PhotoGalleryList
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public List<PhotoGalleryItem> PhotoGallery { get; set; }
    }

    [Serializable]
    public class VideoGalleryItem
    {
        public string Cmp_ID { get; set; }
        public string Std { get; set; }
        public string Division { get; set; }
        public string Title { get; set; }
        public string Video { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
    }

    [Serializable]
    public class VideoGalleryList
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public List<VideoGalleryItem> VideoGallery { get; set; }
    }

    [Serializable]
    public class PDFGalleryItem
    {
        public string Cmp_ID { get; set; }
        public string Std { get; set; }
        public string Division { get; set; }
        public string Title { get; set; }
        public string PDF { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
    }

    [Serializable]
    public class PDFGalleryList
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public List<PDFGalleryItem> PDFGallery { get; set; }
    }

    [Serializable]
    public class ExamItem
    {
        public string Exam_ID { get; set; }
        public string Date { get; set; }
        public string Std { get; set; }
        public string Division { get; set; }
        public string Time { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }
        public string Remarks { get; set; }
        public string Admin_ID { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }

    }

    [Serializable]
    public class Exams
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public List<ExamItem> ListofExams { get; set; }
    }
}
