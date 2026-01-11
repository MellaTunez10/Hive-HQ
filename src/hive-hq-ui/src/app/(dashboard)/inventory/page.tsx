"use client"

import { useQuery } from "@tanstack/react-query"
import { getInventory } from "@/lib/api"
import { InventoryTable } from "@/components/inventory/data-table"
import { columns } from "@/components/inventory/columns"
import { CreateProductModal } from "@/components/inventory/CreateProductModal"
import { Skeleton } from "@/components/ui/skeleton"

export default function InventoryPage() {
    const { data, isLoading, error } = useQuery({
        queryKey: ["inventory"],
        queryFn: getInventory,
    })

    return (
        <div className="container mx-auto py-10 px-4">
            <header className="flex justify-between items-center mb-8">
                <div>
                    <h1 className="text-3xl font-bold tracking-tight text-slate-900">Inventory</h1>
                    <p className="text-slate-500 text-sm">Real-time stock levels and pricing.</p>
                </div>
                <CreateProductModal />
            </header>

            {isLoading ? (
                <div className="space-y-3">
                    <Skeleton className="h-8 w-[250px]" />
                    <Skeleton className="h-[400px] w-full rounded-xl" />
                </div>
            ) : error ? (
                <div className="p-8 text-center border-2 border-red-100 rounded-xl text-red-600 bg-red-50">
                    Connection error: Ensure the .NET API is running at localhost:7250
                </div>
            ) : (
                <InventoryTable columns={columns} data={data || []} />
            )}
        </div>
    )
}
