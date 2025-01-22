<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useRoute, useRouter } from "vue-router";

import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { Content } from "@/types/contents";
import { handleErrorKey } from "@/inject/App";
import { readContent } from "@/api/contents";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();

const content = ref<Content>();

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const value: Content = await readContent(id);
      content.value = value;
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="content">
      <h1>{{ content.invariant.displayName ?? content.invariant.uniqueName }}</h1>
      <StatusDetail :aggregate="content" />
      <!-- TODO(fpion): implement -->
    </template>
  </main>
</template>
