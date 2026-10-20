# Member reference notes

Files here preserve useful original signatures, tokens, reconstruction details and known
differences. Existing source annotations may name them. Their older test counts and status
labels are observations from the time of writing and do not establish current IL parity.

Read a selected type's notes when useful. Obtain the current artifact-bound reference with:

```powershell
python tools/reference/reference.py <Mathcad-type> --assembly <assembly-name> --il
```

The command caches original C#, IL, signatures, SHA-256 and MVID under `.mirror-cache`.
Original code is never committed. New notes are optional; the [five-step workflow](../PLAN.md)
requires only the Release IL report and a short ownership/checkpoint entry.
