
using System.Data;

namespace CostPilot.Common
{
    public static class ValidationConstants
    {
        public static class CostCenter 
        {
            public const int CodeMinLength = 4;
            public const int CodeMaxLength = 4;
            public const int DescriptionMinLength = 2;
            public const int DescriptionMaxLength = 100;
        }

        public static class CostType
        {
            public const int CodeMinLength = 2;
            public const int CodeMaxLength = 2;
            public const int DescriptionMinLength = 2;
            public const int DescriptionMaxLength = 100;
        }

        public static class CostStatus
        {
            public const int DescriptionMinLength = 2;
            public const int DescriptionMaxLength = 50;
        }

        public static class ApplicationUser 
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 50;
        }

        public static class CostCurrency
        { 
            public const int CodeMinLength = 3;
            public const int CodeMaxLength = 3;
        }

        public static class CostRequest 
        {
            public const int NumberMaxLength = 10;
            public const int CommentMaxLength = 500;
            public const int BriefDescriptionMinLength = 2;
            public const int BriefDescriptionMaxLength = 50;
            public const int DetailedDescriptionMinLength = 2;
            public const int DetailedDescriptionMaxLength = 500;
        }
    }
}
