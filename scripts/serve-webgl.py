from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
import argparse
import functools


class UnityWebGLRequestHandler(SimpleHTTPRequestHandler):
    """Serve Unity WebGL gzip artifacts with the headers browsers require."""

    def end_headers(self):
        request_path = self.path.split("?", 1)[0].lower()
        self.send_header("X-Heroic-WebGL-Server", "1")
        if request_path.endswith(".gz"):
            self.send_header("Content-Encoding", "gzip")

        self.send_header("Cross-Origin-Opener-Policy", "same-origin")
        self.send_header("Cross-Origin-Embedder-Policy", "require-corp")
        super().end_headers()

    def guess_type(self, path):
        if path.endswith(".wasm.gz"):
            return "application/wasm"
        if path.endswith(".js.gz"):
            return "application/javascript"
        if path.endswith(".data.gz"):
            return "application/octet-stream"
        return super().guess_type(path)


def main():
    parser = argparse.ArgumentParser(description="Serve the Heroic Unity WebGL build locally.")
    parser.add_argument("--port", type=int, default=5177)
    parser.add_argument("--host", default="127.0.0.1")
    parser.add_argument("--directory", default=str(Path(__file__).resolve().parents[1] / "Builds" / "WebGL"))
    args = parser.parse_args()

    root = Path(args.directory).resolve()
    if not root.exists():
        raise SystemExit(f"WebGL build directory does not exist: {root}")

    handler = functools.partial(UnityWebGLRequestHandler, directory=str(root))
    server = ThreadingHTTPServer((args.host, args.port), handler)
    print(f"Serving Heroic WebGL from {root} at http://{args.host}:{args.port}/")
    server.serve_forever()


if __name__ == "__main__":
    main()
