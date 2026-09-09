.PHONY: check test

check:
	bash -n bin/dotnet-manager install.sh uninstall.sh tests/test.sh
	@if command -v shellcheck >/dev/null 2>&1; then \
		shellcheck bin/dotnet-manager install.sh uninstall.sh tests/test.sh; \
	else \
		echo "shellcheck is not installed; skipping static analysis"; \
	fi

test: check
	./tests/test.sh
