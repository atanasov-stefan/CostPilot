
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
    }
}
