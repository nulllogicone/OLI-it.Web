# Attachment thumbnails and stored script injection

Status: fix prepared in the analysis worktree. Test-slot validation and the cause of the production warning are unverified.

## Data flow

The PostIt and Stamm edit handlers accept a posted `SelectedImageUrl` and can store it as `Datei`. The gallery offers existing uploads, but a caller can submit a different form value. The TopLab edit handler checks the owner's path prefix and some path segments, which still permits JavaScript string delimiters in the filename.

The Stamm PostIt listing loads the account and its messages and renders their attachments through the shared `ImageThumbnail` component. The component accepts absolute HTTP(S) URLs or prefixes relative paths with `ImagesRootUrl`. Its view inserts `ImageUrl` into a single-quoted JavaScript string inside the image's inline `onerror` handler. Razor encodes the apostrophe for HTML. The browser decodes the attribute before interpreting the handler as JavaScript, allowing an apostrophe in the stored value to close the string. A failed image request can then trigger script in the viewing user's app origin.

A local check applied .NET's default HTML encoder to a test value, decoded the resulting handler attribute and evaluated it with Node. The injected assignment ran. This check exercised the encoding and JavaScript boundary in isolation.

## Implementation

- The shared thumbnail uses a plain image with alt text. The gallery click handler reads the blob path from its data attribute.
- A changed gallery selection on Stamm, PostIt or TopLab must exactly match an image in the authenticated owner's blob gallery. The saved value comes from the storage listing. An edit that keeps the current attachment can still save even when the attachment is missing or predates the gallery.
- PostIt supports a separate Direct URL field. Existing absolute HTTP(S) attachments and that entry point still use plain image attributes. Restricting these to the storage container would require a separate decision about external attachments and an inventory of existing values.

## Test-slot checks

Local validation passed the 12 targeted PostIt and TopLab tests, including rejection of a forged gallery selection. An isolated Node check also verified that the gallery treats a path containing an apostrophe as data and clears the upload state on selection.

1. Edit a disposable Stamm, PostIt or TopLab. Upload an image, save, and verify it appears on the detail page and in the gallery when reopening the editor.
2. Select a different existing image, save, and check that it appears after reload. Save a text-only edit and check that the image stays selected.
3. Submit a changed gallery selection using a nonexistent blob under the owner's prefix or an image under another owner's prefix, without an upload. Check that the form shows a validation error and the saved attachment stays the same.
4. Load a missing thumbnail and confirm the rendered image has no inline error handler. Gallery image paths should appear only as data in the click flow.

These checks cover attachment behavior and the identified script path. The production URL's Safe Browsing status is a separate check after deployment.
