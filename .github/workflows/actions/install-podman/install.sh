#!/usr/bin/env bash
curl -fsSL https://download.opensuse.org/repositoriesdevel:/kubic:/libcontainers:/unstable/xUbuntu_$(lsb_release -rs)/Release.key | gpg --dearmor | sudo tee /etc/apt/keyrings/devel_kubic_libcontainers_unstable.gpg > /dev/null
echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/devel_kubic_libcontainers_unstable.gpg] https://download.opensuse.org/repositoriesdevel:/kubic:/libcontainers:/unstable/xUbuntu_$(lsb_release -rs)/ /" | sudo tee /etc/apt/sources.list.d/devel:kubic:libcontainers:unstable.list > /dev/null
sudo apt-get update
sudo apt-get --yes install podman
systemctl enable --now --user podman.socket
echo "DOCKER_HOST=unix://${XDG_RUNTIME_DIR}/podman/podman.sock" >> $GITHUB_ENV
