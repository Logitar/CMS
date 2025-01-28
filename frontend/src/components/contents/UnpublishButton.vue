<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  disabled?: boolean | string;
  loading?: boolean | string;
}>();

const isDisabled = computed<boolean>(() => parseBoolean(props.disabled) || isLoading.value);
const isLoading = computed<boolean>(() => parseBoolean(props.loading) ?? false);

defineEmits<{
  (e: "click"): void;
}>();
</script>

<template>
  <TarButton :disabled="isDisabled" icon="fas fa-mask" :loading="isLoading" :text="t('actions.unpublish')" variant="warning" @click="$emit('click')" />
</template>
