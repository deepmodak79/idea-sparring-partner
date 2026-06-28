#!/bin/sh
set -e

# Render injects PORT (typically 10000). Bind Kestrel to it.
if [ -n "$PORT" ]; then
  export ASPNETCORE_URLS="http://0.0.0.0:${PORT}"
fi

exec dotnet IdeaSparringPartner.Api.dll
