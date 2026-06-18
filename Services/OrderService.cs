using OnlineBookstore.Interfaces;
using OnlineBookstore.Models;
using OnlineBookstore.ViewModels;

namespace OnlineBookstore.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IBookRepository _bookRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IBookRepository bookRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _bookRepository = bookRepository;
        }

        public async Task<Order> CreateOrderAsync(string userId, CheckoutViewModel checkout)
        {
            var cartItems = await _cartRepository.GetCartByUserIdAsync(userId);

            if (!cartItems.Any())
                throw new InvalidOperationException("Cart is empty");

            decimal totalPrice = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var item in cartItems)
            {
                var book = await _bookRepository.GetByIdAsync(item.BookId);
                if (book == null)
                    continue;

                if (book.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Not enough stock for {book.Title}");

                totalPrice += book.Price * item.Quantity;

                orderDetails.Add(new OrderDetail
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = book.Price
                });

                // Update stock
                book.StockQuantity -= item.Quantity;
                await _bookRepository.UpdateAsync(book);
            }

            var order = new Order
            {
                UserId = userId,
                TotalPrice = totalPrice,
                Status = "Pending",
                ShippingAddress = checkout.ShippingAddress,
                ShippingCity = checkout.ShippingCity,
                ShippingPostalCode = checkout.ShippingPostalCode,
                ShippingCountry = checkout.ShippingCountry,
                PaymentMethod = checkout.PaymentMethod,
                OrderDetails = orderDetails
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            // Clear the cart
            await _cartRepository.ClearCartAsync(userId);
            await _cartRepository.SaveChangesAsync();

            return order;
        }

        public async Task<OrderViewModel?> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null) return null;

            return MapToViewModel(order);
        }

        public async Task<List<OrderViewModel>> GetUserOrdersAsync(string userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return orders.Select(MapToViewModel).ToList();
        }

        public async Task<List<OrderViewModel>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(MapToViewModel).ToList();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            if (!await _orderRepository.ExistsAsync(orderId))
                return false;

            await _orderRepository.UpdateStatusAsync(orderId, status);
            await _orderRepository.SaveChangesAsync();
            return true;
        }

        private OrderViewModel MapToViewModel(Order order)
        {
            return new OrderViewModel
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                ShippingCity = order.ShippingCity,
                ShippingCountry = order.ShippingCountry,
                PaymentMethod = order.PaymentMethod,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailViewModel
                {
                    OrderDetailId = od.OrderDetailId,
                    BookId = od.BookId,
                    BookTitle = od.Book?.Title ?? "Unknown",
                    BookAuthor = od.Book?.Author ?? "Unknown",
                    BookImageUrl = od.Book?.ImageUrl,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList() ?? new List<OrderDetailViewModel>()
            };
        }
    }
}
