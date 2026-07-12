"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import { Loader2, MailCheck } from "lucide-react";
import { AuthShell, AuthLink, Field } from "@/components/auth-shell";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { register, activate } from "@/lib/services";

export default function SignupPage() {
  const router = useRouter();
  const [step, setStep] = useState<"details" | "activate">("details");
  const [loading, setLoading] = useState(false);

  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [activationToken, setActivationToken] = useState("");
  const [code, setCode] = useState("");

  async function onRegister(e: React.FormEvent) {
    e.preventDefault();
    setLoading(true);
    try {
      const res = await register({ name, email, password });
      setActivationToken(res.activationToken);
      setStep("activate");
      toast.success("Check your email for the 4-digit activation code.");
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Registration failed");
    } finally {
      setLoading(false);
    }
  }

  async function onActivate(e: React.FormEvent) {
    e.preventDefault();
    setLoading(true);
    try {
      await activate({ activation_token: activationToken, activation_code: code });
      toast.success("Account activated! You can now log in.");
      router.push("/login");
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Activation failed");
    } finally {
      setLoading(false);
    }
  }

  if (step === "activate") {
    return (
      <AuthShell
        title="Verify your email"
        subtitle={`We sent a 4-digit code to ${email}. Enter it below to activate your account.`}
        footer={
          <button className="font-medium text-primary hover:underline" onClick={() => setStep("details")}>
            ← Back to sign up
          </button>
        }
      >
        <form onSubmit={onActivate} className="space-y-5">
          <div className="mx-auto grid h-14 w-14 place-items-center rounded-full bg-primary/15 text-primary">
            <MailCheck className="h-7 w-7" />
          </div>
          <Field label="Activation code">
            <Input
              inputMode="numeric"
              maxLength={4}
              required
              placeholder="1234"
              value={code}
              onChange={(e) => setCode(e.target.value.replace(/\D/g, ""))}
              className="text-center text-2xl tracking-[0.5em]"
            />
          </Field>
          <Button type="submit" className="w-full" disabled={loading || code.length !== 4}>
            {loading ? <Loader2 className="h-4 w-4 animate-spin" /> : "Activate account"}
          </Button>
        </form>
      </AuthShell>
    );
  }

  return (
    <AuthShell
      title="Create your account"
      subtitle="Start learning in minutes — it's free."
      footer={<>Already have an account? <AuthLink href="/login">Log in</AuthLink></>}
    >
      <form onSubmit={onRegister} className="space-y-4">
        <Field label="Full name">
          <Input
            required
            autoComplete="name"
            placeholder="Ada Lovelace"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </Field>
        <Field label="Email">
          <Input
            type="email"
            required
            autoComplete="email"
            placeholder="you@example.com"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </Field>
        <Field label="Password">
          <Input
            type="password"
            required
            minLength={6}
            autoComplete="new-password"
            placeholder="At least 6 characters"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </Field>
        <Button type="submit" className="w-full" disabled={loading}>
          {loading ? <Loader2 className="h-4 w-4 animate-spin" /> : "Create account"}
        </Button>
      </form>
    </AuthShell>
  );
}
