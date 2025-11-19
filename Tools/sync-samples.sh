#!/bin/bash
echo "Synchronizing Samples"

rsync -a --delete -P -h --exclude='*.meta' "./Assets/PackageSamples/" "./Packages/com.spacecow.ecs.anim2d/Samples~/"