"use client";

import { useEffect, useState } from "react";
import { BookOpen, Loader2, ShoppingCart, Users } from "lucide-react";
import { StatCard } from "@/components/admin/stat-card";
import { BarChart } from "@/components/admin/bar-chart";
import { Card, CardContent } from "@/components/ui/card";
import {
  getCoursesAnalytics,
  getOrdersAnalytics,
  getUsersAnalytics,
} from "@/lib/services";
import type { AnalyticsData } from "@/lib/types";

function total(a?: AnalyticsData) {
  return a?.last12Months?.reduce((s, m) => s + m.count, 0) ?? 0;
}

export default function AdminDashboard() {
  const [users, setUsers] = useState<AnalyticsData>();
  const [orders, setOrders] = useState<AnalyticsData>();
  const [courses, setCourses] = useState<AnalyticsData>();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    Promise.all([getUsersAnalytics(), getOrdersAnalytics(), getCoursesAnalytics()])
      .then(([u, o, c]) => {
        setUsers(u);
        setOrders(o);
        setCourses(c);
      })
      .catch((e) => setError(e instanceof Error ? e.message : "Failed to load analytics"))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="grid min-h-[40vh] place-items-center">
        <Loader2 className="h-7 w-7 animate-spin text-primary" />
      </div>
    );
  }

  if (error) return <p className="text-sm text-destructive">{error}</p>;

  return (
    <div className="space-y-6">
      <div className="grid gap-4 sm:grid-cols-3">
        <StatCard
          label="New users (12 mo)"
          value={total(users)}
          icon={<Users className="h-5 w-5" />}
        />
        <StatCard
          label="Orders (12 mo)"
          value={total(orders)}
          icon={<ShoppingCart className="h-5 w-5" />}
        />
        <StatCard
          label="Courses (12 mo)"
          value={total(courses)}
          icon={<BookOpen className="h-5 w-5" />}
        />
      </div>

      <ChartCard title="User signups" subtitle="Last 12 months" data={users} />
      <ChartCard title="Orders" subtitle="Last 12 months" data={orders} />
      <ChartCard title="Courses created" subtitle="Last 12 months" data={courses} />
    </div>
  );
}

function ChartCard({
  title,
  subtitle,
  data,
}: {
  title: string;
  subtitle: string;
  data?: AnalyticsData;
}) {
  return (
    <Card>
      <CardContent>
        <div className="mb-6 flex items-baseline justify-between">
          <h2 className="font-semibold">{title}</h2>
          <span className="text-xs text-muted-foreground">{subtitle}</span>
        </div>
        <BarChart data={data?.last12Months ?? []} />
      </CardContent>
    </Card>
  );
}
