"use client"

import { useState, useEffect } from "react"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
    CardDescription
} from "@/components/ui/card"
import { getInventory } from "@/lib/api"
import { useToast } from "@/hooks/use-toast"
import { Search, ShoppingCart } from "lucide-react"

// Reuse the Product type or define a local subset
type Product = {
    id: string
    name: string
    unitPrice: number
    quantityInStock: number
    reorderLevel: number
    category: string
}

export function OrderCreation() {
    const [query, setQuery] = useState("")
    const [inventory, setInventory] = useState<Product[]>([])
    const [filteredItems, setFilteredItems] = useState<Product[]>([])
    const [loading, setLoading] = useState(true)
    const { toast } = useToast()

    useEffect(() => {
        getInventory()
            .then((data) => {
                setInventory(data)
                setLoading(false)
            })
            .catch((err) => {
                console.error("Failed to load inventory for search", err)
                setLoading(false)
            })
    }, [])

    useEffect(() => {
        if (!query) {
            setFilteredItems([])
            return
        }

        const lowerQuery = query.toLowerCase()
        const results = inventory.filter(item =>
            item.quantityInStock > 0 &&
            ((item.name || "").toLowerCase().includes(lowerQuery) ||
                (item.category || "").toLowerCase().includes(lowerQuery))
        )
        setFilteredItems(results.slice(0, 5)) // Limit to 5 results
    }, [query, inventory])

    const handlePlaceOrder = (item: Product) => {
        // Mock order placement
        toast({
            title: "Order Placed Successfully",
            description: `Order for ${item.name} has been placed.`,
            variant: "default",
            className: "bg-green-600 text-white border-none"
        })
        setQuery("") // Clear search
    }

    const formatCurrency = (amount: number) => {
        return new Intl.NumberFormat("en-NG", {
            style: "currency",
            currency: "NGN",
        }).format(amount)
    }

    return (
        <Card className="mb-6">
            <CardHeader className="pb-3">
                <CardTitle>Create New Order</CardTitle>
                <CardDescription>Search for an item to place a quick order.</CardDescription>
            </CardHeader>
            <CardContent>
                <div className="relative">
                    <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-slate-500" />
                    <Input
                        type="search"
                        placeholder="Search inventory..."
                        className="pl-9"
                        value={query}
                        onChange={(e) => setQuery(e.target.value)}
                    />

                    {/* Search Results Dropdown */}
                    {filteredItems.length > 0 && (
                        <div className="absolute z-10 w-full mt-1 bg-white border border-slate-200 rounded-md shadow-lg overflow-hidden">
                            {filteredItems.map((item) => (
                                <div
                                    key={item.id}
                                    className="flex items-center justify-between p-3 hover:bg-slate-50 cursor-pointer transition-colors"
                                    onClick={() => handlePlaceOrder(item)}
                                >
                                    <div>
                                        <p className="font-medium text-slate-900">{item.name}</p>
                                        <p className="text-xs text-slate-500">{item.category} • {item.quantityInStock} in stock</p>
                                    </div>
                                    <div className="flex items-center gap-4">
                                        <span className="font-semibold text-slate-700">{formatCurrency(item.unitPrice)}</span>
                                        <Button size="sm" variant="secondary" className="h-8 w-8 p-0 rounded-full">
                                            <ShoppingCart className="h-4 w-4" />
                                        </Button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}

                    {query && filteredItems.length === 0 && !loading && (
                        <div className="absolute z-10 w-full mt-1 bg-white border border-slate-200 rounded-md shadow-lg p-3 text-center text-slate-500 text-sm">
                            No items found in stock.
                        </div>
                    )}
                </div>
            </CardContent>
        </Card>
    )
}
