"use client"

import { useMutation, useQueryClient } from "@tanstack/react-query"
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { useState } from "react"

export function CreateProductModal() {
  const queryClient = useQueryClient()
  const [open, setOpen] = useState(false)

  const mutation = useMutation({
    mutationFn: async (newProduct: any) => {
      const res = await fetch("https://localhost:7250/api/Inventory", { // Matches Controller
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(newProduct),
      })
      if (!res.ok) throw new Error("Failed to save item")
      return res.json()
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["inventory"] })
      setOpen(false)
    },
  })

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    const formData = new FormData(e.currentTarget)
    const data = Object.fromEntries(formData)
    
    // Ensure numbers are sent as numbers, not strings
    const payload = {
      name: data.name,
      unitPrice: parseFloat(data.unitPrice as string),
      quantityInStock: parseInt(data.quantityInStock as string),
      reorderLevel: parseInt(data.reorderLevel as string) || 5,
      category: data.category
    }
    
    mutation.mutate(payload)
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button className="bg-blue-600 hover:bg-blue-700">+ Add Item</Button>
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Add New Inventory Item</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <Input name="name" placeholder="Item Name (e.g. A4 Paper)" required />
          <div className="grid grid-cols-2 gap-4">
            <Input name="unitPrice" type="number" step="0.01" placeholder="Unit Price (₦)" required />
            <Input name="quantityInStock" type="number" placeholder="Quantity in Stock" required />
          </div>
          <Input name="category" placeholder="Category (e.g. Stationery)" required />
          <Input name="reorderLevel" type="number" placeholder="Reorder Level (Default: 5)" />
          
          <Button type="submit" className="w-full" disabled={mutation.isPending}>
            {mutation.isPending ? "Saving..." : "Save to Inventory"}
          </Button>
        </form>
      </DialogContent>
    </Dialog>
  )
}