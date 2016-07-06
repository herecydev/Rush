namespace Rush.Azure
{
    public interface IQueueNamer
    {
		string GetQueueName<T>();
    }
}
