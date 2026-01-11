"use client"

import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

const orders = [
    {
        id: "ORD-001",
        customer: "Liam Johnson",
        status: "Fulfilled",
        total: 250000,
        date: "2023-06-25",
    },
    {
        id: "ORD-002",
        customer: "Olivia Smith",
        status: "Pending",
        total: 150000,
        date: "2023-06-26",
    },
    {
        id: "ORD-003",
        customer: "Noah Williams",
        status: "Processing",
        total: 350000,
        date: "2023-06-27",
    },
    {
        id: "ORD-004",
        customer: "Emma Brown",
        status: "Fulfilled",
        total: 450000,
        date: "2023-06-28",
    },
    {
        id: "ORD-005",
        customer: "James Jones",
        status: "Cancelled",
        total: 550000,
        date: "2023-06-29",
    },
]

export function OrdersTable() {
    const formatCurrency = (amount: number) => {
        return new Intl.NumberFormat("en-NG", {
            style: "currency",
            currency: "NGN",
        }).format(amount)
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle>Recent Orders</CardTitle>
            </CardHeader>
            <CardContent>
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead className="w-[100px]">Order</TableHead>
                            <TableHead>Customer</TableHead>
                            <TableHead>Status</TableHead>
                            <TableHead>Date</TableHead>
                            <TableHead className="text-right">Amount</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {orders.map((order) => (
                            <TableRow key={order.id}>
                                <TableCell className="font-medium">{order.id}</TableCell>
                                <TableCell>{order.customer}</TableCell>
                                <TableCell>
                                    <Badge
                                        className={
                                            order.status === "Fulfilled"
                                                ? "bg-green-500 hover:bg-green-600"
                                                : order.status === "Pending"
                                                    ? "bg-yellow-500 hover:bg-yellow-600 text-black"
                                                    : order.status === "Processing"
                                                        ? "bg-blue-500 hover:bg-blue-600"
                                                        : "bg-red-500 hover:bg-red-600"
                                        }
                                    >
                                        {order.status}
                                    </Badge>
                                </TableCell>
                                <TableCell>{order.date}</TableCell>
                                <TableCell className="text-right">{formatCurrency(order.total)}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </CardContent>
        </Card>
    )
}
