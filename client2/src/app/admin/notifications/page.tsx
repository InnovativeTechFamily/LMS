"use client";

import { useEffect, useState } from "react";
import { toast } from "sonner";
import { BellRing, Check, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { getNotifications, markNotificationRead } from "@/lib/services";
import { formatDate } from "@/lib/utils";
import type { Notification } from "@/lib/types";

export default function AdminNotificationsPage() {
  const [items, setItems] = useState<Notification[] | null>(null);
  const [busyId, setBusyId] = useState<string | null>(null);

  useEffect(() => {
    getNotifications()
      .then(setItems)
      .catch((e) => {
        toast.error(e instanceof Error ? e.message : "Failed to load notifications");
        setItems([]);
      });
  }, []);

  async function markRead(id: string) {
    setBusyId(id);
    try {
      const updated = await markNotificationRead(id);
      setItems(updated);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : "Could not update");
    } finally {
      setBusyId(null);
    }
  }

  const unread = items?.filter((n) => n.status === "unread").length ?? 0;

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-semibold">Notifications</h2>
          <p className="text-sm text-muted-foreground">
            {unread} unread · {items?.length ?? 0} total
          </p>
        </div>
      </div>

      {items === null ? (
        <div className="grid min-h-[30vh] place-items-center">
          <Loader2 className="h-6 w-6 animate-spin text-primary" />
        </div>
      ) : items.length === 0 ? (
        <div className="grid place-items-center rounded-2xl border border-dashed border-border py-16 text-center">
          <BellRing className="mb-3 h-8 w-8 text-muted-foreground" />
          <p className="text-sm text-muted-foreground">You&apos;re all caught up.</p>
        </div>
      ) : (
        <div className="space-y-3">
          {items.map((n) => (
            <Card key={n._id} className={n.status === "unread" ? "border-primary/40" : undefined}>
              <CardContent className="flex items-start justify-between gap-4 p-4">
                <div className="flex gap-3">
                  <div
                    className={`mt-0.5 grid h-9 w-9 shrink-0 place-items-center rounded-full ${
                      n.status === "unread"
                        ? "bg-primary/15 text-primary"
                        : "bg-secondary text-muted-foreground"
                    }`}
                  >
                    <BellRing className="h-4 w-4" />
                  </div>
                  <div>
                    <div className="flex items-center gap-2">
                      <p className="text-sm font-medium">{n.title}</p>
                      {n.status === "unread" && (
                        <span className="rounded-full bg-primary/15 px-2 py-0.5 text-[10px] font-medium text-primary">
                          New
                        </span>
                      )}
                    </div>
                    <p className="text-sm text-muted-foreground">{n.message}</p>
                    <p className="mt-1 text-xs text-muted-foreground">{formatDate(n.createdAt)}</p>
                  </div>
                </div>
                {n.status === "unread" && (
                  <Button
                    variant="ghost"
                    size="sm"
                    disabled={busyId === n._id}
                    onClick={() => markRead(n._id)}
                  >
                    {busyId === n._id ? (
                      <Loader2 className="h-4 w-4 animate-spin" />
                    ) : (
                      <>
                        <Check className="h-4 w-4" /> Mark read
                      </>
                    )}
                  </Button>
                )}
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
