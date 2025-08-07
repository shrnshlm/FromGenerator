export interface FormField {
  name: string;
  label: string;
  type: 'text' | 'email' | 'tel' | 'date' | 'number' | 'textarea' | 'select' | 'radio' | 'checkbox';
  required: boolean;
  placeholder: string;
  options: string[];
  value: string;
}

export interface GeneratedForm {
  formId: string;
  title: string;
  intent: string;
  fields: FormField[];
  submitUrl: string;
  submitButtonText: string;
}

export interface FormGenerationRequest {
  text: string;
  userId?: string;
}

export interface FormSubmissionRequest {
  formId: string;
  fieldValues: Record<string, string>;
}

export interface FormSubmissionResponse {
  success: boolean;
  message: string;
  formId: string;
  submittedAt: string;
}

export interface ErrorResponse {
  error: string;
  details: string;
  timestamp: string;
}

export interface ApiResponse<T> {
  data?: T;
  error?: ErrorResponse;
}