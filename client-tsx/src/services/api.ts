import type {
  FormGenerationRequest,
  FormSubmissionRequest,
  GeneratedForm,
  FormSubmissionResponse,
  ErrorResponse
} from '../types/api';

// const API_BASE_URL = 'https://vbi6kae6z7ic7pfurd6zjkzahu0isjdc.lambda-url.us-west-2.on.aws';

class ApiError extends Error {
  public error: ErrorResponse;
  
  constructor(error: ErrorResponse) {
    super(error.error);
    this.name = 'ApiError';
    this.error = error;
  }
}

class ApiService {
  // Temporarily unused while using mock implementation
  // private async makeRequest<T>(
  //   endpoint: string,
  //   options: RequestInit = {}
  // ): Promise<T> {
  //   const url = `${API_BASE_URL}${endpoint}`;
    
  //   const response = await fetch(url, {
  //     headers: {
  //       'Content-Type': 'application/json',
  //       ...options.headers,
  //     },
  //     ...options,
  //   });

  //   if (!response.ok) {
  //     let errorData: ErrorResponse;
  //     try {
  //       errorData = await response.json();
  //     } catch {
  //       errorData = {
  //         error: 'Network Error',
  //         details: `HTTP ${response.status}: ${response.statusText}`,
  //         timestamp: new Date().toISOString(),
  //       };
  //     }
  //     throw new ApiError(errorData);
  //   }

  //   return response.json();
  // }

  async generateForm(request: FormGenerationRequest): Promise<GeneratedForm> {
    // Temporary mock implementation while backend is being fixed
    await new Promise(resolve => setTimeout(resolve, 1000)); // Simulate API delay

    // Simple intent detection based on keywords
    const text = request.text.toLowerCase();
    let formType = 'contact';
    let title = 'Contact Form';
    let fields: any[] = [];

    if (text.includes('flight') || text.includes('book') || text.includes('travel')) {
      formType = 'flight';
      title = 'Flight Booking';
      fields = [
        { name: 'departure', label: 'Departure City', type: 'text', required: true, placeholder: 'Enter departure city', value: '' },
        { name: 'destination', label: 'Destination City', type: 'text', required: true, placeholder: 'Enter destination city', value: '' },
        { name: 'departureDate', label: 'Departure Date', type: 'date', required: true, value: '' },
        { name: 'passengers', label: 'Number of Passengers', type: 'number', required: true, placeholder: '1', value: '1' },
        { name: 'class', label: 'Travel Class', type: 'select', required: true, options: ['Economy', 'Business', 'First Class'], value: 'Economy' }
      ];
    } else if (text.includes('hotel') || text.includes('room') || text.includes('reservation')) {
      formType = 'hotel';
      title = 'Hotel Reservation';
      fields = [
        { name: 'city', label: 'City', type: 'text', required: true, placeholder: 'Enter city', value: '' },
        { name: 'checkIn', label: 'Check-in Date', type: 'date', required: true, value: '' },
        { name: 'checkOut', label: 'Check-out Date', type: 'date', required: true, value: '' },
        { name: 'guests', label: 'Number of Guests', type: 'number', required: true, value: '1' },
        { name: 'roomType', label: 'Room Type', type: 'select', required: true, options: ['Standard', 'Deluxe', 'Suite'], value: 'Standard' }
      ];
    } else if (text.includes('register') || text.includes('signup') || text.includes('join')) {
      formType = 'registration';
      title = 'Registration';
      fields = [
        { name: 'firstName', label: 'First Name', type: 'text', required: true, placeholder: 'Enter your first name', value: '' },
        { name: 'lastName', label: 'Last Name', type: 'text', required: true, placeholder: 'Enter your last name', value: '' },
        { name: 'email', label: 'Email Address', type: 'email', required: true, placeholder: 'Enter your email', value: '' },
        { name: 'phone', label: 'Phone Number', type: 'tel', required: false, placeholder: 'Enter your phone number', value: '' }
      ];
    } else {
      // Default contact form
      fields = [
        { name: 'name', label: 'Full Name', type: 'text', required: true, placeholder: 'Enter your full name', value: '' },
        { name: 'email', label: 'Email Address', type: 'email', required: true, placeholder: 'Enter your email', value: '' },
        { name: 'message', label: 'Message', type: 'textarea', required: true, placeholder: 'Enter your message', value: request.text }
      ];
    }

    const form: GeneratedForm = {
      formId: Math.random().toString(36).substr(2, 9),
      title,
      intent: formType,
      fields,
      submitUrl: '/api/form/submit',
      submitButtonText: 'Submit'
    };

    return form;
  }

  async submitForm(request: FormSubmissionRequest): Promise<FormSubmissionResponse> {
    // Temporary mock implementation
    await new Promise(resolve => setTimeout(resolve, 500)); // Simulate API delay

    return {
      success: true,
      message: 'Form submitted successfully! (This is a demo)',
      formId: request.formId,
      submittedAt: new Date().toISOString()
    };
  }
}

export const apiService = new ApiService();
export { ApiError };