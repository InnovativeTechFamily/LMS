"use client";

import { useEffect, useState } from "react";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";
import { Table, TD, TH, THead, TRow } from "@/components/ui/table";
import { getAdminCourses, getOrders, getUsers } from "@/lib/services";
import { formatDate } from "@/lib/utils";
import type { Course, Order, User } from "@/lib/types";

export default function AdminOrdersPage() {
  const [orders, setOrders] = useState<Order[] | null>(null);
  const [courses, setCourses] = useState<Map<string, Course>>(new Map());
  const [users, setUsers] = useState<Map<string, User>>(new Map());

  useEffect(() => {
    Promise.all([getOrders(), getAdminCourses(), getUsers()])
      .then(([o, c, u]) => {
        setOrders(o);
        setCourses(new Map(c.map((x) => [x._id, x])));
        setUsers(new Map(u.map((x) => [x._id, x])));
      })
      .catch((e) => {
        toast.error(e instanceof Error ? e.message : "Failed to load orders");
        setOrders([]);
      });
  }, []);

  return (
    <div className="space-y-5">
      <div>
        <h2 className="text-lg font-semibold">Orders</h2>
        <p className="text-sm text-muted-foreground">{orders?.length ?? 0} total</p>
      </div>

      {orders === null ? (
        <div className="grid min-h-[30vh] place-items-center">
          <Loader2 className="h-6 w-6 animate-spin text-primary" />
        </div>
      ) : (
        <Table>
          <THead>
            <TRow>
              <TH>Order</TH>
              <TH>Customer</TH>
              <TH>Course</TH>
              <TH>Date</TH>
            </TRow>
          </THead>
          <tbody>
            {orders.map((o) => {
              const user = users.get(o.userId);
              const course = courses.get(o.courseId);
              return (
                <TRow key={o._id}>
                  <TD className="font-mono text-xs text-muted-foreground">
                    #{o._id.slice(-8)}
                  </TD>
                  <TD>
                    <p className="font-medium">{user?.name ?? "Unknown"}</p>
                    <p className="text-xs text-muted-foreground">{user?.email ?? o.userId}</p>
                  </TD>
                  <TD>{course?.name ?? o.courseId}</TD>
                  <TD className="text-muted-foreground">{formatDate(o.createdAt)}</TD>
                </TRow>
              );
            })}
            {orders.length === 0 && (
              <TRow>
                <TD colSpan={4} className="py-10 text-center text-muted-foreground">
                  No orders yet.
                </TD>
              </TRow>
            )}
          </tbody>
        </Table>
      )}
    </div>
  );
}
