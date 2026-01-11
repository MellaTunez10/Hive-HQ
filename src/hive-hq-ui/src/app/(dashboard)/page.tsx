"use client"
import DashboardStats from "@/components/DashboardStats";
import { OrdersTable } from "@/components/orders/OrdersTable";
import { OrderCreation } from "@/components/orders/OrderCreation";

export default function Home() {
  return (
    <div className="container mx-auto py-10">
      <DashboardStats />
      <div className="mt-10">
        <OrderCreation />
        <OrdersTable />
      </div>
    </div>
  )
}