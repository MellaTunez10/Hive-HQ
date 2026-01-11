"use client"

import { ColumnDef } from "@tanstack/react-table"
import { Badge } from "@/components/ui/badge"

export type Product = {
  id: string
  name: string
  price: number
  quantityInStock: number
  category: string
  reorderLevel: number
}

export const columns: ColumnDef<Product>[] = [
  {
    accessorKey: "name",
    header: "Product Name",
  },
  {
    accessorKey: "category",
    header: "Category",
    cell: ({ row }) => <Badge variant="outline">{row.getValue("category")}</Badge>
  },
  {
    accessorKey: "unitPrice", // Ensure this matches your JSON key exactly
    header: "Unit Price",
    cell: ({ row }) => {
      // Logic to handle potential undefined or string values
      const value = row.getValue("unitPrice");
      const amount = typeof value === "number" ? value : parseFloat(value as string);
      
      if (isNaN(amount)) return "₦0.00";

      return new Intl.NumberFormat("en-NG", {
        style: "currency",
        currency: "NGN",
      }).format(amount);
    },
  },
  {
    accessorKey: "quantityInStock", // Corrected
    header: "In Stock",
    cell: ({ row }) => {
      const qty = row.original.quantityInStock
      const lowStock = qty <= row.original.reorderLevel
      return (
        <span className={lowStock ? "text-red-500 font-bold" : ""}>
          {qty} units {lowStock && "(Low)"}
        </span>
      )
    },
  },
]