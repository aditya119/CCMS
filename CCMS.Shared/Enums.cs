namespace CCMS.Shared.Enums
{
    public static class ProceedingDecisions
    {
        public const string Pending = "PENDING";
        public const string Adjournment = "ADJOURNMENT";
        public const string InterimOrder = "INTERIM ORDER";
        public const string FinalJudgement = "FINAL JUDGEMENT";
    }
    public static class Roles
    {
        public const string Operator = "Operator";
        public const string Manager = "Manager";
        public const string Administrator = "Administrator";
        public const int OperatorInt = 1;
        public const int ManagerInt = 2;
        public const int AdministratorInt = 4;
        public const int MinValue = OperatorInt;
        public const int MaxValue = OperatorInt | ManagerInt | AdministratorInt;
    }
    public static class CaseActors
    {
        public const int Petitioner = 1;
        public const int Respondent = 2;
        public const int MinValue = Petitioner;
        public const int MaxValue = Respondent;
    }
    public static class Platforms
    {
        public const int Web = 1;
        public const int Desktop = 2;
        public const int Mobile = 3;
        public const int MinValue = Web;
        public const int MaxValue = Mobile;
    }
}
