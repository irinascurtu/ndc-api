using OrdersApi.Data.Domain;

namespace OrdersApi.Models
{
    public class OrderModel
    {

        public OrderModel()
        {
            Status = OrderStatus.Created;
        }
        public OrderStatus Status { get; set; }

        //customer things
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        //Shipping things

        public List<OrderItemModel> OrderItems { get; set; }
    }
}
