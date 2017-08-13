using SQLite;

namespace MathFighter.Model
{
    public class Subject
    {
        [PrimaryKey, NotNull]
        public int SubjectID { get; set; }  
        public string SubjectName { get; set; }

        public Subject ()
        { 
            
        }

        public Subject(int subjectId, string subjectName)
        {
            this.SubjectID = subjectId;
            this.SubjectName = subjectName;
        }

    }
}   