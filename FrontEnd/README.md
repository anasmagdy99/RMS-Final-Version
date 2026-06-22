# RMS Leave Management System - Frontend

A modern, enterprise-grade React frontend for the RMS Leave Management System with a stunning red gradient theme.

## 🎨 Design Features

### Color Palette
- **Primary**: Red gradient from #C4161C to #E53935
- **Background**: Clean light gray (#F8F9FB) with white cards
- **Typography**: Inter, Poppins, Roboto (modern sans-serif)
- **Components**: Cards with rounded corners, soft shadows, and smooth animations

### UI Components
- ✅ Responsive sidebar navigation with role-based menu items
- ✅ Dashboard with statistics cards and ag-Grid charts
- ✅ ag-Grid tables with filtering, sorting, and grouping
- ✅ Modal dialogs for forms and details
- ✅ Status badges with color coding
- ✅ Loading states and error handling

## 🚀 Tech Stack

- **Framework**: React 18 + Vite
- **Routing**: React Router DOM v6
- **State Management**: React Context API
- **HTTP Client**: Axios
- **Data Grid**: ag-Grid React (Community & Enterprise)
- **Charts**: ag-Charts React
- **Styling**: Vanilla CSS with CSS Variables

## 📁 Project Structure

```
FrontEnd/
├── src/
│   ├── components/          # Reusable UI components
│   │   ├── Sidebar.jsx
│   │   ├── Header.jsx
│   │   ├── Card.jsx
│   │   ├── Loading.jsx
│   │   └── ProtectedRoute.jsx
│   ├── pages/              # Page components
│   │   ├── Login.jsx
│   │   ├── Dashboard.jsx
│   │   ├── MyRequests.jsx
│   │   ├── LeaveBalance.jsx
│   │   └── Profile.jsx
│   ├── services/           # API services
│   │   ├── api.js
│   │   ├── authService.js
│   │   ├── transactionService.js
│   │   ├── dashboardService.js
│   │   ├── leaveBalanceService.js
│   │   └── employeeService.js
│   ├── context/            # React Context
│   │   └── AuthContext.jsx
│   ├── theme/              # Theme configuration
│   │   ├── colors.js
│   │   ├── typography.js
│   │   ├── layout.js
│   │   └── index.js
│   ├── App.jsx             # Main app component
│   ├── main.jsx            # Entry point
│   └── index.css           # Global styles
├── .env                    # Environment variables
├── index.html              # HTML template
├── package.json
└── vite.config.js
```

## 🔧 Installation

1. **Install dependencies**:
   ```bash
   npm install
   ```

2. **Configure environment**:
   Update `.env` file with your backend API URL:
   ```
   VITE_API_BASE_URL=https://localhost:5001/api
   ```

3. **Run development server**:
   ```bash
   npm run dev
   ```

4. **Build for production**:
   ```bash
   npm run build
   ```

## 🔐 Authentication

The app uses a custom authentication system with:
- Employee code and password login
- JWT-like token storage in localStorage
- Automatic header injection for API requests
- Role-based route protection

### Login Credentials (from backend seeding)
- **HR Manager**: Code `1980009`, Password `Pass#123`
- **Employee**: Code `1990027`, Password `Pass#123`

## 👥 Role-Based Features

### Employee Role
- ✅ Dashboard (personal stats)
- ✅ My Requests (create, edit, cancel)
- ✅ Leave Balance (personal)
- ✅ Profile

### Manager Role (+ Employee features)
- ✅ Team Requests (approve/reject team members)
- ✅ Team Leave Balances

### HR Role (+ Manager features)
- ✅ All Requests (company-wide)
- ✅ Employee Management (CRUD)
- ✅ All Leave Balances
- ✅ Advanced filtering and reporting

### Board Role
- ✅ History (all requests)
- ✅ HR Requests
- ✅ Dashboard with full analytics

## 📊 Features

### Dashboard
- Real-time statistics cards
- Bar charts (requests by department/status)
- Pie charts (distribution)
- Filters: Date range, department, employee
- Red-themed ag-Charts

### My Requests
- ag-Grid table with pagination
- Inline actions (View, Edit, Cancel)
- Status badges
- Create new requests
- Edit pending requests
- Cancel before approval

### Leave Balance
- Personal balance cards
- Detailed accrual information
- Team/All employees table (role-based)
- Historical balance calculation

### Profile
- Employee information
- Department and role details
- Manager status indicator

## 🎯 API Integration

All services are fully integrated with the backend:
- Authentication: `/api/auth/login`
- Transactions: `/api/transactions/*`
- Dashboard: `/api/dashboard/*`
- Leave Balance: `/api/leavebalance/*`
- Employees: `/api/employees/*`

Headers automatically injected:
- `X-Employee-Id`: Current user ID
- `X-Employee-Role`: Current user role

## 🎨 Theme Customization

Theme tokens are centralized in `/src/theme/`:
- **colors.js**: Color palette
- **typography.js**: Font families and sizes
- **layout.js**: Spacing, shadows, border radius

CSS variables in `index.css` for easy customization.

## 📱 Responsive Design

- Desktop-first approach
- Sidebar collapses on mobile
- Cards stack vertically
- Tables become scrollable
- Touch-friendly buttons

## 🚦 Development

### Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

### Code Style

- Functional components with hooks
- Context API for global state
- Custom hooks for reusable logic
- CSS modules for component styles
- Semantic HTML5

## 🔒 Security

- Protected routes with role checking
- Automatic token refresh
- XSS protection
- HTTPS only in production
- Environment variable for sensitive data

## 📈 Performance

- Code splitting with React.lazy
- Optimized bundle size
- Lazy loading for routes
- Memoization for expensive computations
- Virtual scrolling in ag-Grid

## 🎯 Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## 📝 License

Proprietary - RMS Leave Management System

## 👨‍💻 Development Team

Built with ❤️ by the RMS Development Team
