"use client";

import { useEffect, useState } from "react";
import { Loader2, VideoOff } from "lucide-react";
import { getVideoOtp } from "@/lib/services";

/**
 * Renders a lecture video. When `videoUrl` is a full http(s) URL it is embedded directly
 * (YouTube/Vimeo/mp4). Otherwise it is treated as a VdoCipher video id and played via
 * an OTP fetched from the backend, mirroring the original Node/VdoCipher flow.
 */
export function VideoPlayer({ videoUrl }: { videoUrl?: string }) {
  const [embed, setEmbed] = useState<{ otp: string; playbackInfo: string } | null>(null);
  const [status, setStatus] = useState<"idle" | "loading" | "error">("idle");

  const isDirectUrl = !!videoUrl && /^https?:\/\//i.test(videoUrl);

  useEffect(() => {
    setEmbed(null);
    if (!videoUrl || isDirectUrl) {
      setStatus("idle");
      return;
    }
    let cancelled = false;
    setStatus("loading");
    getVideoOtp(videoUrl)
      .then((res) => {
        if (!cancelled) {
          setEmbed(res);
          setStatus("idle");
        }
      })
      .catch(() => {
        if (!cancelled) setStatus("error");
      });
    return () => {
      cancelled = true;
    };
  }, [videoUrl, isDirectUrl]);

  const frame = "absolute inset-0 h-full w-full border-0";

  return (
    <div className="relative aspect-video w-full overflow-hidden rounded-2xl border border-border bg-black">
      {!videoUrl ? (
        <Placeholder message="No video attached to this lecture yet." />
      ) : isDirectUrl ? (
        <iframe
          className={frame}
          src={videoUrl}
          allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture; fullscreen"
          allowFullScreen
          title="Lecture video"
        />
      ) : status === "loading" ? (
        <div className="grid h-full w-full place-items-center">
          <Loader2 className="h-8 w-8 animate-spin text-white/80" />
        </div>
      ) : status === "error" ? (
        <Placeholder message="This video couldn't be loaded right now." />
      ) : embed ? (
        <iframe
          className={frame}
          src={`https://player.vdocipher.com/v2/?otp=${embed.otp}&playbackInfo=${embed.playbackInfo}`}
          allow="encrypted-media"
          allowFullScreen
          title="Lecture video"
        />
      ) : null}
    </div>
  );
}

function Placeholder({ message }: { message: string }) {
  return (
    <div className="grid h-full w-full place-items-center bg-brand-gradient/10 text-center">
      <div className="flex flex-col items-center gap-2 text-white/70">
        <VideoOff className="h-8 w-8" />
        <p className="max-w-xs px-6 text-sm">{message}</p>
      </div>
    </div>
  );
}
