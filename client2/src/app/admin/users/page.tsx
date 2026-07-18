"use client";

import { useEffect, useState } from "react";
import { toast } from "sonner";
import { Loader2, Search, Trash2 } from "lucide-react";
import { Avatar } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Modal } from "@/components/ui/modal";
import { Table, TD, TH, THead, TRow } from "@/components/ui/table";
import { deleteUser, getUsers, updateUserRole } from "@/lib/services";
import { formatDate } from "@/lib/utils";
import { useAuth } from "@/providers/auth-provider";
import type { User } from "@/lib/types";

export default function AdminUsersPage() {
  const { user: me } = useAuth();
  const [users, setUsers] = useState<User[] | null>(null);
  const [query, setQuery] = useState("");
  const [busyId, setBusyId] = useState<string | null>(null);
  const [toDelete, setToDelete] = useState<User | null>(null);

  async function load() {
    try {
      setUsers(await getUsers());
    } catch (e) {
      toast.error(e instanceof Error ? e.message : "Failed to load users");
      setUsers([]);
    }
  }

  useEffect(() => {
    load();
  }, []);

  async function changeRole(u: User, role: string) {
    setBusyId(u._id);
    try {
      await updateUserRole({ email: u.email, role });
      setUsers((prev) => prev?.map((x) => (x._id === u._id ? { ...x, role } : x)) ?? null);
      toast.success(`${u.name} is now ${role}.`);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : "Could not update role");
    } finally {
      setBusyId(null);
    }
  }

  async function confirmDelete() {
    if (!toDelete) return;
    setBusyId(toDelete._id);
    try {
      await deleteUser(toDelete._id);
      setUsers((prev) => prev?.filter((x) => x._id !== toDelete._id) ?? null);
      toast.success("User deleted.");
      setToDelete(null);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : "Could not delete user");
    } finally {
      setBusyId(null);
    }
  }

  const filtered =
    users?.filter(
      (u) =>
        u.name.toLowerCase().includes(query.toLowerCase()) ||
        u.email.toLowerCase().includes(query.toLowerCase())
    ) ?? [];

  return (
    <div className="space-y-5">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h2 className="text-lg font-semibold">Users</h2>
          <p className="text-sm text-muted-foreground">{users?.length ?? 0} total</p>
        </div>
        <div className="relative">
          <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Search users…"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            className="w-64 pl-9"
          />
        </div>
      </div>

      {users === null ? (
        <Loader />
      ) : (
        <Table>
          <THead>
            <TRow>
              <TH>User</TH>
              <TH>Courses</TH>
              <TH>Joined</TH>
              <TH>Role</TH>
              <TH className="text-right">Actions</TH>
            </TRow>
          </THead>
          <tbody>
            {filtered.map((u) => (
              <TRow key={u._id}>
                <TD>
                  <div className="flex items-center gap-3">
                    <Avatar name={u.name} url={u.avatar?.url} className="h-9 w-9 text-xs" />
                    <div className="min-w-0">
                      <p className="truncate font-medium">{u.name}</p>
                      <p className="truncate text-xs text-muted-foreground">{u.email}</p>
                    </div>
                  </div>
                </TD>
                <TD className="text-muted-foreground">{u.courses?.length ?? 0}</TD>
                <TD className="text-muted-foreground">{formatDate(u.createdAt)}</TD>
                <TD>
                  <select
                    value={u.role}
                    disabled={busyId === u._id || u._id === me?._id}
                    onChange={(e) => changeRole(u, e.target.value)}
                    className="rounded-lg border border-input bg-background px-2.5 py-1.5 text-sm capitalize focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:opacity-60"
                  >
                    <option value="user">user</option>
                    <option value="admin">admin</option>
                  </select>
                </TD>
                <TD className="text-right">
                  <Button
                    variant="ghost"
                    size="icon"
                    aria-label="Delete user"
                    disabled={busyId === u._id || u._id === me?._id}
                    onClick={() => setToDelete(u)}
                    className="text-muted-foreground hover:text-destructive"
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </TD>
              </TRow>
            ))}
            {filtered.length === 0 && (
              <TRow>
                <TD colSpan={5} className="py-10 text-center text-muted-foreground">
                  No users found.
                </TD>
              </TRow>
            )}
          </tbody>
        </Table>
      )}

      <Modal open={!!toDelete} onClose={() => setToDelete(null)} title="Delete user">
        <p className="text-sm text-muted-foreground">
          Are you sure you want to permanently delete <span className="font-medium text-foreground">{toDelete?.name}</span>?
          This cannot be undone.
        </p>
        <div className="mt-6 flex justify-end gap-2">
          <Button variant="ghost" onClick={() => setToDelete(null)}>
            Cancel
          </Button>
          <Button variant="destructive" onClick={confirmDelete} disabled={busyId === toDelete?._id}>
            {busyId === toDelete?._id ? <Loader2 className="h-4 w-4 animate-spin" /> : "Delete"}
          </Button>
        </div>
      </Modal>
    </div>
  );
}

function Loader() {
  return (
    <div className="grid min-h-[30vh] place-items-center">
      <Loader2 className="h-6 w-6 animate-spin text-primary" />
    </div>
  );
}
