import type {
  FormGenerationRequest,
  FormSubmissionRequest,
  GeneratedForm,
  FormSubmissionResponse,
  ErrorResponse
} from '../types/api';

// Configuration for different backends
const BACKENDS = {
  microservices: 'http://localhost:7000', // API Gateway for microservices
  monolithic: 'http://localhost:5000',    // Direct monolithic backend
  aws: 'https://vbi6kae6z7ic7pfurd6zjkzahu0isjdc.lambda-url.us-west-2.on.aws' // AWS Lambda
};

// Choose backend - can be made configurable via environment variable
const API_BASE_URL = BACKENDS.microservices;

class ApiError extends Error {
  public error: ErrorResponse;
  
  constructor(error: ErrorResponse) {
    super(error.error);
    this.name = 'ApiError';
    this.error = error;
  }
}

class ApiService {
  private async makeRequest<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${API_BASE_URL}${endpoint}`;
    
    const response = await fetch(url, {
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
      ...options,
    });

    if (!response.ok) {
      let errorData: ErrorResponse;
      try {
        errorData = await response.json();
      } catch {
        errorData = {
          error: 'Network Error',
          details: `HTTP ${response.status}: ${response.statusText}`,
          timestamp: new Date().toISOString(),
        };
      }
      throw new ApiError(errorData);
    }

    return response.json();
  }

  async generateForm(request: FormGenerationRequest): Promise<GeneratedForm> {
    // Call the microservices API Gateway -> Form Generation Service
    return this.makeRequest<GeneratedForm>('/form-generation/api/form/generate', {
      method: 'POST',
      body: JSON.stringify(request),
    });
  }

  async submitForm(request: FormSubmissionRequest): Promise<FormSubmissionResponse> {
    // Call the microservices API Gateway -> Form Submission Service
    return this.makeRequest<FormSubmissionResponse>('/form-submission/api/form/submit', {
      method: 'POST',
      body: JSON.stringify(request),
    });
  }
}

export const apiService = new ApiService();
export { ApiError };