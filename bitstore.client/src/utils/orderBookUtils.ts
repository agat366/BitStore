interface OrderItem {
  price: number;
  amount: number;
}

interface OrderBar {
  avgPrice: number;
  totalAmount: number;
  percentage: number;
}

export function aggregateOrders(orders: OrderItem[], groupCount: number = 10): OrderBar[] {
  if (!orders.length) return [];

  // Sort orders by price (ascending for asks, descending for bids)
  const sortedOrders = [...orders].sort((a, b) => a.price - b.price);

  // Calculate group size
  const groupSize = Math.ceil(orders.length / groupCount);
  const result: OrderBar[] = [];

  // Create groups
  for (let i = 0; i < groupCount; i++) {
    const start = i * groupSize;
    const group = sortedOrders.slice(start, start + groupSize);

    if (group.length === 0) break;

    // Calculate aggregated values for the group
    const totalAmount = group.reduce((sum, order) => sum + order.amount, 0);
    const avgPrice = Math.round(
      group.reduce((sum, order) => sum + order.price * order.amount, 0) / totalAmount
    );    result.push({
      avgPrice,
      totalAmount,
      percentage: 0 // Will be calculated after all groups are processed
    });
  }

  // Calculate percentages based on the maximum amount
  const maxAmount = Math.max(...result.map(bar => bar.totalAmount));
  result.forEach(bar => {
    bar.percentage = (bar.totalAmount / maxAmount) * 100;
  });

  return result;
}
