using Lab_3_4.Events;

namespace Lab_3_4.Services
{
    public interface IRabbitMqService
    {
        void Publish(CarEvent carEvent);
    }
}
