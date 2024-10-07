using garage87.Enums.Attributes;
using System.ComponentModel;

namespace garage87.Enums
{
    public enum RepairStatusEnum
    {
        [Heading("Not Started")]
        NotStarted = 1,

        [Heading("In Progress")]
        InProgress = 2,


        [Heading("Completed")]
        Completed = 3,

        [Heading("Cancelled")]
        Cancelled = 4,
    }
}
