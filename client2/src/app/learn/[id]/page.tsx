"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { useParams } from "next/navigation";
import Link from "next/link";
import { toast } from "sonner";
import {
  ArrowLeft,
  CheckCircle2,
  Link2,
  Loader2,
  Lock,
  MessageCircle,
  PlayCircle,
  Send,
  Star,
} from "lucide-react";
import { RequireAuth } from "@/components/guard";
import { VideoPlayer } from "@/components/video-player";
import { Avatar } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Ratings } from "@/components/ratings";
import { ApiError } from "@/lib/api";
import {
  addAnswer,
  addQuestion,
  addReview,
  getCourse,
  getCourseContent,
} from "@/lib/services";
import { cn, formatDate, initials } from "@/lib/utils";
import { useAuth } from "@/providers/auth-provider";
import type { Course, CourseData } from "@/lib/types";

type Tab = "overview" | "qa" | "reviews";

export default function LearnPage() {
  return (
    <RequireAuth>
      <LearnInner />
    </RequireAuth>
  );
}

function LearnInner() {
  const { id } = useParams<{ id: string }>();
  const [content, setContent] = useState<CourseData[] | null>(null);
  const [course, setCourse] = useState<Course | null>(null);
  const [active, setActive] = useState(0);
  const [tab, setTab] = useState<Tab>("overview");
  const [locked, setLocked] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadContent = useCallback(async () => {
    const c = await getCourseContent(id);
    setContent(c);
  }, [id]);

  useEffect(() => {
    getCourse(id).then(setCourse).catch(() => {});
    loadContent().catch((e) => {
      if (e instanceof ApiError && (e.status === 400 || e.status === 403)) setLocked(true);
      else setError(e instanceof Error ? e.message : "Failed to load course");
    });
  }, [id, loadContent]);

  const sections = useMemo(() => groupBySection(content ?? []), [content]);

  if (locked) {
    return (
      <div className="container grid min-h-[60vh] place-items-center py-20 text-center">
        <div className="max-w-sm">
          <div className="mx-auto mb-5 grid h-14 w-14 place-items-center rounded-full bg-primary/15 text-primary">
            <Lock className="h-7 w-7" />
          </div>
          <h1 className="text-xl font-semibold">You&apos;re not enrolled yet</h1>
          <p className="mt-2 text-sm text-muted-foreground">
            Enroll in this course to unlock all lectures.
          </p>
          <Button asChild className="mt-6">
            <Link href={`/course/${id}`}>View course</Link>
          </Button>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container grid min-h-[60vh] place-items-center py-20 text-center">
        <p className="text-muted-foreground">{error}</p>
      </div>
    );
  }

  if (!content) {
    return (
      <div className="grid min-h-[60vh] place-items-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  const lecture = content[active];

  return (
    <div className="container py-8">
      <Link
        href="/my-courses"
        className="mb-4 inline-flex items-center gap-1.5 text-sm text-muted-foreground hover:text-foreground"
      >
        <ArrowLeft className="h-4 w-4" /> My Learning
      </Link>

      <div className="grid gap-8 lg:grid-cols-[1fr_340px]">
        {/* Player + tabs */}
        <div className="min-w-0">
          <VideoPlayer videoUrl={lecture?.videoUrl} />

          <h1 className="mt-5 text-xl font-bold tracking-tight md:text-2xl">
            {lecture?.title ?? course?.name}
          </h1>

          <div className="mt-5 flex gap-1 border-b border-border">
            <TabButton active={tab === "overview"} onClick={() => setTab("overview")}>
              Overview
            </TabButton>
            <TabButton active={tab === "qa"} onClick={() => setTab("qa")}>
              Q&amp;A
              {lecture?.questions?.length ? (
                <span className="ml-1.5 rounded-full bg-secondary px-1.5 text-xs">
                  {lecture.questions.length}
                </span>
              ) : null}
            </TabButton>
            <TabButton active={tab === "reviews"} onClick={() => setTab("reviews")}>
              Reviews
            </TabButton>
          </div>

          <div className="py-6">
            {tab === "overview" && <Overview lecture={lecture} />}
            {tab === "qa" && lecture && (
              <QASection
                courseId={id}
                lecture={lecture}
                onChanged={loadContent}
              />
            )}
            {tab === "reviews" && (
              <ReviewsSection
                courseId={id}
                course={course}
                onChanged={() => getCourse(id).then(setCourse)}
              />
            )}
          </div>
        </div>

        {/* Curriculum */}
        <aside className="lg:sticky lg:top-20 lg:self-start">
          <div className="overflow-hidden rounded-2xl border border-border bg-card">
            <div className="border-b border-border px-4 py-3">
              <p className="text-sm font-semibold">Course content</p>
              <p className="text-xs text-muted-foreground">{content.length} lectures</p>
            </div>
            <div className="max-h-[70vh] overflow-y-auto">
              {sections.map((section) => (
                <div key={section.name}>
                  {section.name && (
                    <p className="bg-secondary/40 px-4 py-2 text-xs font-medium uppercase tracking-wide text-muted-foreground">
                      {section.name}
                    </p>
                  )}
                  {section.items.map(({ item, index }) => (
                    <button
                      key={item._id}
                      onClick={() => {
                        setActive(index);
                        setTab("overview");
                      }}
                      className={cn(
                        "flex w-full items-center gap-3 border-b border-border/50 px-4 py-3 text-left transition-colors last:border-0",
                        index === active ? "bg-primary/10" : "hover:bg-secondary/40"
                      )}
                    >
                      {index === active ? (
                        <PlayCircle className="h-4 w-4 shrink-0 text-primary" />
                      ) : (
                        <CheckCircle2 className="h-4 w-4 shrink-0 text-muted-foreground/40" />
                      )}
                      <span className="min-w-0 flex-1">
                        <span className="line-clamp-2 text-sm">{item.title}</span>
                        {item.videoLength ? (
                          <span className="text-xs text-muted-foreground">
                            {item.videoLength} min
                          </span>
                        ) : null}
                      </span>
                    </button>
                  ))}
                </div>
              ))}
            </div>
          </div>
        </aside>
      </div>
    </div>
  );
}

function groupBySection(content: CourseData[]) {
  const order: string[] = [];
  const map = new Map<string, { item: CourseData; index: number }[]>();
  content.forEach((item, index) => {
    const key = item.videoSection ?? "";
    if (!map.has(key)) {
      map.set(key, []);
      order.push(key);
    }
    map.get(key)!.push({ item, index });
  });
  return order.map((name) => ({ name, items: map.get(name)! }));
}

function TabButton({
  active,
  onClick,
  children,
}: {
  active: boolean;
  onClick: () => void;
  children: React.ReactNode;
}) {
  return (
    <button
      onClick={onClick}
      className={cn(
        "-mb-px flex items-center border-b-2 px-4 py-2.5 text-sm font-medium transition-colors",
        active
          ? "border-primary text-foreground"
          : "border-transparent text-muted-foreground hover:text-foreground"
      )}
    >
      {children}
    </button>
  );
}

function Overview({ lecture }: { lecture?: CourseData }) {
  if (!lecture) return null;
  return (
    <div className="space-y-6">
      {lecture.description && (
        <p className="whitespace-pre-line text-sm leading-relaxed text-muted-foreground">
          {lecture.description}
        </p>
      )}
      {lecture.links?.length > 0 && (
        <div>
          <h3 className="mb-2 text-sm font-semibold">Resources</h3>
          <ul className="space-y-2">
            {lecture.links.map((l, i) => (
              <li key={i}>
                <a
                  href={l.url}
                  target="_blank"
                  rel="noreferrer"
                  className="inline-flex items-center gap-2 text-sm text-primary hover:underline"
                >
                  <Link2 className="h-4 w-4" /> {l.title || l.url}
                </a>
              </li>
            ))}
          </ul>
        </div>
      )}
      {!lecture.description && !lecture.links?.length && (
        <p className="text-sm text-muted-foreground">No overview for this lecture.</p>
      )}
    </div>
  );
}

function QASection({
  courseId,
  lecture,
  onChanged,
}: {
  courseId: string;
  lecture: CourseData;
  onChanged: () => Promise<void>;
}) {
  const [question, setQuestion] = useState("");
  const [busy, setBusy] = useState(false);
  const [replyTo, setReplyTo] = useState<string | null>(null);
  const [answer, setAnswer] = useState("");

  async function submitQuestion(e: React.FormEvent) {
    e.preventDefault();
    if (!question.trim()) return;
    setBusy(true);
    try {
      await addQuestion({ question: question.trim(), courseId, contentId: lecture._id });
      setQuestion("");
      await onChanged();
      toast.success("Question posted.");
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Could not post question");
    } finally {
      setBusy(false);
    }
  }

  async function submitAnswer(questionId: string) {
    if (!answer.trim()) return;
    setBusy(true);
    try {
      await addAnswer({ answer: answer.trim(), courseId, contentId: lecture._id, questionId });
      setAnswer("");
      setReplyTo(null);
      await onChanged();
      toast.success("Reply posted.");
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Could not post reply");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="space-y-6">
      <form onSubmit={submitQuestion} className="space-y-3">
        <Textarea
          placeholder="Ask a question about this lecture…"
          value={question}
          onChange={(e) => setQuestion(e.target.value)}
        />
        <Button type="submit" disabled={busy || !question.trim()} size="sm">
          {busy ? <Loader2 className="h-4 w-4 animate-spin" /> : <>Post question <Send className="h-4 w-4" /></>}
        </Button>
      </form>

      <div className="space-y-4">
        {lecture.questions?.length ? (
          lecture.questions.map((q) => (
            <div key={q._id} className="rounded-2xl border border-border bg-card p-4">
              <div className="flex gap-3">
                <Avatar name={q.user?.name} url={q.user?.avatar?.url} className="h-9 w-9 shrink-0 text-xs" />
                <div className="min-w-0 flex-1">
                  <div className="flex items-center gap-2">
                    <span className="text-sm font-medium">{q.user?.name ?? "Learner"}</span>
                    <span className="text-xs text-muted-foreground">{formatDate(q.createdAt)}</span>
                  </div>
                  <p className="mt-1 text-sm">{q.question}</p>

                  {q.questionReplies?.length > 0 && (
                    <div className="mt-3 space-y-3 border-l-2 border-border pl-4">
                      {q.questionReplies.map((r) => (
                        <div key={r._id} className="flex gap-2.5">
                          <Avatar
                            name={r.user?.name}
                            url={r.user?.avatar?.url}
                            className="h-7 w-7 shrink-0 text-[10px]"
                          />
                          <div>
                            <div className="flex items-center gap-2">
                              <span className="text-xs font-medium">{r.user?.name ?? "User"}</span>
                              {r.user?.role === "admin" && (
                                <span className="rounded bg-primary/15 px-1.5 text-[10px] font-medium text-primary">
                                  Instructor
                                </span>
                              )}
                            </div>
                            <p className="text-sm text-muted-foreground">{r.answer}</p>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}

                  {replyTo === q._id ? (
                    <div className="mt-3 space-y-2">
                      <Textarea
                        placeholder="Write a reply…"
                        value={answer}
                        onChange={(e) => setAnswer(e.target.value)}
                        className="min-h-[70px]"
                      />
                      <div className="flex gap-2">
                        <Button size="sm" onClick={() => submitAnswer(q._id)} disabled={busy}>
                          {busy ? <Loader2 className="h-4 w-4 animate-spin" /> : "Reply"}
                        </Button>
                        <Button size="sm" variant="ghost" onClick={() => setReplyTo(null)}>
                          Cancel
                        </Button>
                      </div>
                    </div>
                  ) : (
                    <button
                      onClick={() => {
                        setReplyTo(q._id);
                        setAnswer("");
                      }}
                      className="mt-2 inline-flex items-center gap-1.5 text-xs font-medium text-primary hover:underline"
                    >
                      <MessageCircle className="h-3.5 w-3.5" /> Reply
                    </button>
                  )}
                </div>
              </div>
            </div>
          ))
        ) : (
          <p className="text-sm text-muted-foreground">
            No questions yet. Be the first to ask!
          </p>
        )}
      </div>
    </div>
  );
}

function ReviewsSection({
  courseId,
  course,
  onChanged,
}: {
  courseId: string;
  course: Course | null;
  onChanged: () => void;
}) {
  const { user } = useAuth();
  const [rating, setRating] = useState(5);
  const [review, setReview] = useState("");
  const [busy, setBusy] = useState(false);

  const alreadyReviewed = !!course?.reviews?.some((r) => r.user?._id === user?._id);

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    if (!review.trim()) return;
    setBusy(true);
    try {
      await addReview(courseId, { review: review.trim(), rating });
      setReview("");
      onChanged();
      toast.success("Thanks for your review!");
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Could not post review");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="space-y-6">
      {!alreadyReviewed && (
        <form onSubmit={submit} className="space-y-3 rounded-2xl border border-border bg-card p-4">
          <p className="text-sm font-medium">Leave a review</p>
          <div className="flex items-center gap-1">
            {Array.from({ length: 5 }).map((_, i) => (
              <button key={i} type="button" onClick={() => setRating(i + 1)} aria-label={`${i + 1} stars`}>
                <Star
                  className={cn(
                    "h-6 w-6 transition-colors",
                    i + 1 <= rating ? "fill-yellow-400 text-yellow-400" : "text-muted-foreground/40"
                  )}
                />
              </button>
            ))}
          </div>
          <Textarea
            placeholder="Share what you think about this course…"
            value={review}
            onChange={(e) => setReview(e.target.value)}
          />
          <Button type="submit" size="sm" disabled={busy || !review.trim()}>
            {busy ? <Loader2 className="h-4 w-4 animate-spin" /> : "Submit review"}
          </Button>
        </form>
      )}

      <div className="space-y-4">
        {course?.reviews?.length ? (
          course.reviews.map((r) => (
            <div key={r._id} className="rounded-2xl border border-border bg-card p-5">
              <div className="flex items-center gap-3">
                <span className="grid h-10 w-10 place-items-center rounded-full bg-brand-gradient text-sm font-semibold text-white">
                  {initials(r.user?.name)}
                </span>
                <div>
                  <p className="text-sm font-medium">{r.user?.name ?? "Learner"}</p>
                  <Ratings value={r.rating} />
                </div>
              </div>
              <p className="mt-3 text-sm text-muted-foreground">{r.comment}</p>
              {r.commentReplies?.map((cr) => (
                <div key={cr._id} className="mt-3 border-l-2 border-border pl-4">
                  <p className="text-xs font-medium">{cr.user?.name ?? "Instructor"}</p>
                  <p className="text-sm text-muted-foreground">{cr.comment}</p>
                </div>
              ))}
            </div>
          ))
        ) : (
          <p className="text-sm text-muted-foreground">No reviews yet.</p>
        )}
      </div>
    </div>
  );
}
