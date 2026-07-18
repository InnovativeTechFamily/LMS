"use client";

import { useRef, useState } from "react";
import { toast } from "sonner";
import { Camera, KeyRound, Loader2, UserCog } from "lucide-react";
import { RequireAuth } from "@/components/guard";
import { Avatar } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent } from "@/components/ui/card";
import { Field } from "@/components/auth-shell";
import { useAuth } from "@/providers/auth-provider";
import { updateAvatar, updatePassword, updateUserInfo } from "@/lib/services";
import { readFileAsDataUrl } from "@/lib/utils";

type Tab = "info" | "password";

export default function ProfilePage() {
  return (
    <RequireAuth>
      <ProfileInner />
    </RequireAuth>
  );
}

function ProfileInner() {
  const { user, refresh } = useAuth();
  const [tab, setTab] = useState<Tab>("info");
  const fileRef = useRef<HTMLInputElement>(null);
  const [uploading, setUploading] = useState(false);

  async function onAvatar(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    setUploading(true);
    try {
      const dataUrl = await readFileAsDataUrl(file);
      await updateAvatar(dataUrl);
      await refresh();
      toast.success("Profile photo updated.");
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Upload failed");
    } finally {
      setUploading(false);
      if (fileRef.current) fileRef.current.value = "";
    }
  }

  return (
    <div className="relative">
      <div className="absolute inset-x-0 top-0 -z-10 h-64 bg-brand-radial" />
      <div className="container max-w-4xl py-12">
        {/* Header */}
        <div className="flex flex-col items-center gap-4 text-center sm:flex-row sm:text-left">
          <div className="relative">
            <Avatar name={user?.name} url={user?.avatar?.url} className="h-24 w-24 text-2xl" />
            <button
              onClick={() => fileRef.current?.click()}
              disabled={uploading}
              className="absolute -bottom-1 -right-1 grid h-9 w-9 place-items-center rounded-full border-2 border-background bg-primary text-white shadow-lg transition hover:brightness-110 disabled:opacity-60"
              aria-label="Change photo"
            >
              {uploading ? <Loader2 className="h-4 w-4 animate-spin" /> : <Camera className="h-4 w-4" />}
            </button>
            <input ref={fileRef} type="file" accept="image/*" hidden onChange={onAvatar} />
          </div>
          <div>
            <h1 className="text-2xl font-bold tracking-tight">{user?.name}</h1>
            <p className="text-sm text-muted-foreground">{user?.email}</p>
            <div className="mt-2 flex items-center justify-center gap-2 sm:justify-start">
              <span className="rounded-full bg-primary/15 px-2.5 py-0.5 text-xs font-medium capitalize text-primary">
                {user?.role}
              </span>
              <span className="text-xs text-muted-foreground">
                {user?.courses?.length ?? 0} enrolled course
                {(user?.courses?.length ?? 0) === 1 ? "" : "s"}
              </span>
            </div>
          </div>
        </div>

        {/* Tabs */}
        <div className="mt-10 flex gap-2 border-b border-border">
          <TabButton active={tab === "info"} onClick={() => setTab("info")} icon={<UserCog className="h-4 w-4" />}>
            Account
          </TabButton>
          <TabButton
            active={tab === "password"}
            onClick={() => setTab("password")}
            icon={<KeyRound className="h-4 w-4" />}
          >
            Password
          </TabButton>
        </div>

        <div className="mt-6">
          {tab === "info" ? <InfoForm /> : <PasswordForm />}
        </div>
      </div>
    </div>
  );
}

function TabButton({
  active,
  onClick,
  icon,
  children,
}: {
  active: boolean;
  onClick: () => void;
  icon: React.ReactNode;
  children: React.ReactNode;
}) {
  return (
    <button
      onClick={onClick}
      className={`-mb-px flex items-center gap-2 border-b-2 px-4 py-2.5 text-sm font-medium transition-colors ${
        active
          ? "border-primary text-foreground"
          : "border-transparent text-muted-foreground hover:text-foreground"
      }`}
    >
      {icon}
      {children}
    </button>
  );
}

function InfoForm() {
  const { user, refresh } = useAuth();
  const [name, setName] = useState(user?.name ?? "");
  const [saving, setSaving] = useState(false);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!name.trim()) return;
    setSaving(true);
    try {
      await updateUserInfo({ name: name.trim() });
      await refresh();
      toast.success("Profile updated.");
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Update failed");
    } finally {
      setSaving(false);
    }
  }

  return (
    <Card>
      <CardContent>
        <form onSubmit={onSubmit} className="max-w-md space-y-4">
          <Field label="Full name">
            <Input value={name} onChange={(e) => setName(e.target.value)} required />
          </Field>
          <Field label="Email">
            <Input value={user?.email ?? ""} disabled />
          </Field>
          <Button type="submit" disabled={saving || name.trim() === user?.name}>
            {saving ? <Loader2 className="h-4 w-4 animate-spin" /> : "Save changes"}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}

function PasswordForm() {
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirm, setConfirm] = useState("");
  const [saving, setSaving] = useState(false);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (newPassword !== confirm) {
      toast.error("New passwords do not match.");
      return;
    }
    setSaving(true);
    try {
      await updatePassword({ oldPassword, newPassword });
      toast.success("Password changed.");
      setOldPassword("");
      setNewPassword("");
      setConfirm("");
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Update failed");
    } finally {
      setSaving(false);
    }
  }

  return (
    <Card>
      <CardContent>
        <form onSubmit={onSubmit} className="max-w-md space-y-4">
          <Field label="Current password">
            <Input
              type="password"
              autoComplete="current-password"
              value={oldPassword}
              onChange={(e) => setOldPassword(e.target.value)}
              required
            />
          </Field>
          <Field label="New password">
            <Input
              type="password"
              autoComplete="new-password"
              minLength={6}
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              required
            />
          </Field>
          <Field label="Confirm new password">
            <Input
              type="password"
              autoComplete="new-password"
              minLength={6}
              value={confirm}
              onChange={(e) => setConfirm(e.target.value)}
              required
            />
          </Field>
          <Button type="submit" disabled={saving}>
            {saving ? <Loader2 className="h-4 w-4 animate-spin" /> : "Update password"}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
