namespace SchoolApp.Exceptions
{
    public class EntityNotAuthorizedException : Exception
    {
        public EntityNotAuthorizedException() { }

        public EntityNotAuthorizedException(string message) : base(message) { }

        public EntityNotAuthorizedException(string message, Exception inner) : base(message, inner) { }
    }
}
