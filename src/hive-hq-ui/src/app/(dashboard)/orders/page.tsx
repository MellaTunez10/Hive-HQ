"use client"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

export default function OrdersPage() {
    return (
        <div className="container mx-auto py-10 px-4">
            <header className="mb-8">
                <h1 className="text-3xl font-bold tracking-tight text-slate-900">Orders</h1>
                <p className="text-slate-500 text-sm">Manage customer orders and shipments.</p>
            </header>

            <div className="grid gap-4">
                <Card>
                    <CardHeader>
                        <CardTitle>Recent Orders</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-center text-slate-500 py-8">
                            No orders found. (Orders data integration coming soon)
                        </div>
                    </CardContent>
                </Card>
            </div>
        </div>
    )
}
